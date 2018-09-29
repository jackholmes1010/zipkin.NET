using System;
using Moq;
using Xunit;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Logging;
using Zipkin.NET.Sampling;

namespace Zipkin.NET.Tests
{
    public class TracerTests
    {
        [Fact]
        public void GetContextAccessor_TracerNotStarted()
        {
            Tracer.Stop();
            Assert.Throws<Exception>(() => Tracer.ContextAccessor);
        }

        [Fact]
        public void GetSampler_TracerNotStarted()
        {
            Tracer.Stop();
            Assert.Throws<Exception>(() => Tracer.Sampler);
        }

        [Fact]
        public void GetLogger_TracerNotStarted()
        {
            Tracer.Stop();
            Assert.Throws<Exception>(() => Tracer.Logger);
        }

        [Fact]
        public void StartTracer_Success()
        {
            var sampler = new Mock<Sampler>();
            var accessor = new Mock<ITraceContextAccessor>();
            var dispatcher = new Mock<Dispatcher>(accessor.Object);
            var logger = new Mock<IInstrumentationLogger>();

            Tracer.Start(
                sampler.Object, 
                dispatcher.Object, 
                accessor.Object, 
                logger.Object);

            Assert.True(Tracer.Started);
        }
    }
}
