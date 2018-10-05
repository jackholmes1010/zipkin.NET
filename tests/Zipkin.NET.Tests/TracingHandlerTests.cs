using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Xunit;
using Zipkin.NET.Constants;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Models;
using Zipkin.NET.Sampling;
using Zipkin.NET.Tests.Util;

namespace Zipkin.NET.Tests
{
    [ExcludeFromCodeCoverage]
    public class TracingHandlerTests
    {
        private readonly Mock<ITraceContextAccessor> _mockTraceContextAccessor;
        private readonly Mock<IDispatcher> _mockDispatcher;
        private readonly Mock<ISampler> _mockSampler;
        private readonly IFixture _fixture;

        public TracingHandlerTests()
        {
            _mockTraceContextAccessor = new Mock<ITraceContextAccessor>();
            _mockDispatcher = new Mock<IDispatcher>();
            _mockSampler = new Mock<ISampler>();
            _fixture = new Fixture();
        }

        public static IEnumerable<object[]> CtorArgs()
        {
            yield return new object[] { null, new Mock<IDispatcher>().Object, new Mock<ISampler>().Object, "test" };
            yield return new object[] { new Mock<ITraceContextAccessor>().Object, null, new Mock<ISampler>().Object, "test" };
            yield return new object[] { new Mock<ITraceContextAccessor>().Object, new Mock<IDispatcher>().Object, null, "test" };
            yield return new object[] { new Mock<ITraceContextAccessor>().Object, new Mock<IDispatcher>().Object, new Mock<ISampler>().Object, null };
            
        }

        [Theory]
        [MemberData(nameof(CtorArgs))]
        public void Ctor_NullArgs(
            ITraceContextAccessor traceContextAccessor,
            IDispatcher dispatcher,
            ISampler sampler,
            string remoteEndpointName)
        {
            Assert.Throws<ArgumentNullException>(
                () => new TracingHandler(
                    traceContextAccessor,
                    dispatcher,
                    sampler,
                    remoteEndpointName));
        }

        [Theory]
        [MemberData(nameof(CtorArgs))]
        public void Ctor_NullArgsInnerHandler(
            ITraceContextAccessor traceContextAccessor,
            IDispatcher dispatcher,
            ISampler sampler,
            string remoteEndpointName)
        {
            Assert.Throws<ArgumentNullException>(
                () => new TracingHandler(
                    new HttpClientHandler(),
                    traceContextAccessor,
                    dispatcher,
                    sampler,
                    remoteEndpointName));
        }

        [Fact]
        public async Task SendAsync_Success()
        {
            var traceContextFixture = _fixture.Create<TraceContext>();

            _mockTraceContextAccessor.Setup(t => t.HasTrace()).Returns(true);
            _mockTraceContextAccessor.Setup(t => t.GetTrace()).Returns(traceContextFixture);

            _mockSampler.Setup(
                    s => s.IsSampled(It.Is<TraceContext>(t => t.TraceId == traceContextFixture.TraceId)))
                .Returns(true);

            _mockDispatcher.Setup(
                d => d.Dispatch(
                    It.IsAny<Span>(), 
                    It.Is<TraceContext>(t => t.TraceId == traceContextFixture.TraceId)));

            var tracingHandler = new TracingHandler(
                new DummyHandler(AssertPropagationHeadersAddedToRequest),
                _mockTraceContextAccessor.Object,
                _mockDispatcher.Object,
                _mockSampler.Object,
                "test");

            var httpClient = new HttpClient(tracingHandler);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri("http://test.com"));
            var response = await httpClient.SendAsync(httpRequest);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            _mockTraceContextAccessor.VerifyAll();
            _mockSampler.VerifyAll();
            _mockDispatcher.VerifyAll();
        }

        /// <summary>
        /// If an exception is thrown in the pipeline following the TracingHandler, the span
        /// should still be dispatched. The dispatched span should contain the exception message.
        /// </summary>
        [Fact]
        public async Task SendAsync_InnerHandlerException()
        {
            var traceContextFixture = _fixture.Create<TraceContext>();
            var exceptionFixture = new Exception(_fixture.Create<string>());

            _mockTraceContextAccessor.Setup(t => t.HasTrace()).Returns(true);
            _mockTraceContextAccessor.Setup(t => t.GetTrace()).Returns(traceContextFixture);

            _mockSampler.Setup(
                    s => s.IsSampled(It.Is<TraceContext>(t => t.TraceId == traceContextFixture.TraceId)))
                .Returns(true);

            _mockDispatcher.Setup(
                d => d.Dispatch(
                    It.Is<Span>(s => s.Tags.ContainsKey("error") && s.Tags["error"] == exceptionFixture.Message),
                    It.Is<TraceContext>(t => t.TraceId == traceContextFixture.TraceId)));

            var tracingHandler = new TracingHandler(
                new ExceptionDummyHandler(AssertPropagationHeadersAddedToRequest, exceptionFixture), 
                _mockTraceContextAccessor.Object,
                _mockDispatcher.Object,
                _mockSampler.Object,
                "test");

            var httpClient = new HttpClient(tracingHandler);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri("http://test.com"));

            try
            {
                await httpClient.SendAsync(httpRequest);
            }
            catch (Exception)
            {
                // ignored
            }

            _mockTraceContextAccessor.VerifyAll();
            _mockSampler.VerifyAll();
            _mockDispatcher.VerifyAll();
        }

        private static void AssertPropagationHeadersAddedToRequest(HttpRequestMessage request)
        {
            AssertHeaderAddedToRequest(request, B3HeaderConstants.TraceId);
            AssertHeaderAddedToRequest(request, B3HeaderConstants.SpanId);
            AssertHeaderAddedToRequest(request, B3HeaderConstants.ParentSpanId);
            AssertHeaderAddedToRequest(request, B3HeaderConstants.Sampled);
            AssertHeaderAddedToRequest(request, B3HeaderConstants.Flags);
        }

        private static void AssertHeaderAddedToRequest(HttpRequestMessage request, string header)
        {
            Assert.True(request.Headers.TryGetValues(header, out var traceHeaders));
            Assert.NotNull(traceHeaders);
            Assert.True(traceHeaders.Count() == 1);
        }
    }
}
