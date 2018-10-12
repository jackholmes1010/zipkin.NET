using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Xunit;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Logging;
using Zipkin.NET.Models;
using Zipkin.NET.Reporters;

namespace Zipkin.NET.Tests.DispatcherTests
{
    [ExcludeFromCodeCoverage]
    public class AsyncActionBlockDispatcherTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IReporter> _mockReporter1;
        private readonly Mock<IReporter> _mockReporter2;
        private readonly Mock<IInstrumentationLogger> _mockInstrumentationLogger;

        public AsyncActionBlockDispatcherTests()
        {
            _fixture = new Fixture();
            _mockReporter1 = new Mock<IReporter>();
            _mockReporter2 = new Mock<IReporter>();
            _mockInstrumentationLogger = new Mock<IInstrumentationLogger>();
        }

        [Fact]
        public void Dispatch_Success()
        {
            var spanContext = _fixture.Create<SpanContext>();
            spanContext.Sampled = true;
            var span = new Span(spanContext);

            SetupMockReporters(span);

            var dispatcher = new AsyncActionBlockDispatcher(
                new []{_mockReporter1.Object, _mockReporter2.Object},
                _mockInstrumentationLogger.Object);

            dispatcher.Dispatch(span);

            // Wait for processing of spans to complete
            while (!dispatcher.IsCompleted())
            {
                Thread.Sleep(50);
            }

            _mockReporter1.VerifyAll();
            _mockReporter2.VerifyAll();
        }

        /// <summary>
        /// If the SpanContext Sampled property is null should throw an exception.
        /// </summary>
        [Fact]
        public void Dispatch_SpanContextSampledNull()
        {
            var spanContext = _fixture.Create<SpanContext>();
            spanContext.Sampled = null;
            var span = new Span(spanContext);

            SetupMockReporters(span);

            var dispatcher = new AsyncActionBlockDispatcher(
                new[] { _mockReporter1.Object, _mockReporter2.Object },
                _mockInstrumentationLogger.Object);

            Assert.Throws<Exception>(() => dispatcher.Dispatch(span));
        }

        /// <summary>
        /// If the SpanContext Sampled property is false, the dispatcher should not report the span.
        /// </summary>
        [Fact]
        public void Dispatch_SpanContextSampledFalse()
        {
            var span = _fixture.Create<Span>();
            var trace = _fixture.Create<SpanContext>();
            trace.Sampled = false;

            SetupMockReporters(span);

            var dispatcher = new AsyncActionBlockDispatcher(
                new[] { _mockReporter1.Object, _mockReporter2.Object },
                _mockInstrumentationLogger.Object);

            dispatcher.Dispatch(span);

            // Wait for processing of spans to complete
            while (!dispatcher.IsCompleted())
            {
                Thread.Sleep(50);
            }

            _mockReporter1.Verify(r => r.ReportAsync(It.IsAny<Span>()), Times.Never);
            _mockReporter2.Verify(r => r.ReportAsync(It.IsAny<Span>()), Times.Never);
        }

        private void SetupMockReporters(Span span = null)
        {
            _mockReporter1
                .Setup(r => r.ReportAsync(It.Is<Span>(s => s == span)))
                .Returns(Task.CompletedTask);

            _mockReporter2
                .Setup(r => r.ReportAsync(It.Is<Span>(s => s == span)))
                .Returns(Task.CompletedTask);
        }
    }
}
