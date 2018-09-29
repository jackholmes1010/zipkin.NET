using System;
using System.Collections.Generic;
using System.Text;
using AutoFixture;
using Moq;
using Xunit;
using Zipkin.NET.Logging;
using Zipkin.NET.Models;
using Zipkin.NET.Reporters;
using Zipkin.NET.Sampling;

namespace Zipkin.NET.Tests
{
    public class TracerTests
    {
        private readonly IFixture _fixture;

        public TracerTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void GetContextAccessor_TracerNotStarted()
        {
            Assert.Throws<Exception>(() => Tracer.ContextAccessor);
        }

        [Fact]
        public void GetSampler_TracerNotStarted()
        {
            Assert.Throws<Exception>(() => Tracer.Sampler);
        }

        [Fact]
        public void GetLogger_TracerNotStarted()
        {
            Assert.Throws<Exception>(() => Tracer.Logger);
        }

        [Fact]
        public void Report_TracerNotStarted()
        {
            var traceContext = _fixture.Create<TraceContext>();
            var span = _fixture.Create<Span>();
            Assert.Throws<Exception>(() => Tracer.Report(traceContext, span));
        }

        [Fact]
        public void Report_SampledPropertyNotSet()
        {
            var traceContext = _fixture.Create<TraceContext>();
            var span = _fixture.Create<Span>();

            var sampler = new Mock<Sampler>();
            var traceContextAccessor = new Mock<ITraceContextAccessor>();
            var instrumentationLogger = new Mock<IInstrumentationLogger>();
            var reporter = new Mock<IReporter>();
            var reporters = new[] {reporter.Object};

            Tracer.Start(
                sampler.Object, 
                traceContextAccessor.Object,
                instrumentationLogger.Object,
                reporters);

            traceContext.Sampled = null;

            Assert.Throws<Exception>(() => Tracer.Report(traceContext, span));
        }
    }
}
