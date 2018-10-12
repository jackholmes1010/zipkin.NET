using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AutoFixture;
using Xunit;
using Zipkin.NET.Sampling;

namespace Zipkin.NET.Tests.SamplerTests
{
    [ExcludeFromCodeCoverage]
    public class RateSamplerTests
    {
        private readonly IFixture _fixture;

        public RateSamplerTests()
        {
            _fixture = new Fixture();
        }

        /// <summary>
        /// A rate sampler with a sampling rate of 1f (100%) should sample all traces.
        /// </summary>
        [Theory]
        [InlineData(1f)]
        [InlineData(2f)]
        [InlineData(100f)]
        public void IsSampled_SampleEverything(float rate)
        {
            var rateSampler = new RateSampler(rate);

            for (var i = 0; i < 1000; i++)
            {
                var traceContext = _fixture.Create<SpanContext>();
                traceContext.Debug = false;
                traceContext.Sampled = null;

                var sampled = rateSampler.IsSampled(traceContext);
                Assert.True(sampled);
            }
        }

        /// <summary>
        /// A rate sampler with a sampling rate of 0f (0%) should not sample any traces.
        /// </summary>
        [Theory]
        [InlineData(0f)]
        [InlineData(-1f)]
        [InlineData(-0.001f)]
        public void IsSampled_NeverSample(float rate)
        {
            var rateSampler = new RateSampler(rate);

            for (var i = 0; i < 1000; i++)
            {
                var traceContext = _fixture.Create<SpanContext>();
                traceContext.Debug = false;
                traceContext.Sampled = null;

                var sampled = rateSampler.IsSampled(traceContext);
                Assert.False(sampled);
            }
        }

        [Theory]
        [InlineData(0.1f)]
        [InlineData(0.2f)]
        [InlineData(0.01f)]
        [InlineData(0.001f)]
        [InlineData(0.0001f)]
        public void IsSampled_SpecificSampleRate(float rate)
        {
            var rateSampler = new RateSampler(rate);
            var decisions = new List<bool>();

            for (var i = 0; i < 10000; i++)
            {
                var traceContext = _fixture.Create<SpanContext>();
                traceContext.Debug = false;
                traceContext.Sampled = null;

                var sampled = rateSampler.IsSampled(traceContext);
                decisions.Add(sampled);
            }

            var expectedSampleCount = rate * 10000;
            var sampleDecisions = decisions.Where(d => d);

            Assert.Equal(expectedSampleCount, sampleDecisions.Count());
        }

        /// <summary>
        /// If the span context debug property is true, the Sampler should always sample.
        /// </summary>
        [Fact]
        public void IsSampled_TraceContextDebug()
        {
            var rateSampler = new RateSampler(0f);
            var traceContext = _fixture.Create<SpanContext>();
            traceContext.Debug = true;
            traceContext.Sampled = null;

            var sampled = rateSampler.IsSampled(traceContext);
            Assert.True(sampled);
        }

        /// <summary>
        /// If the span context sampled property is not null, this means a sampling decision has already been made.
        /// The sampler should respect the existing sampling decision.
        /// </summary>
        [Theory]
        [InlineData(0f, true)]
        [InlineData(0.5f, false)]
        [InlineData(1f, false)]
        [InlineData(0.5f, true)]
        public void IsSampled_SamplingDecisionAlreadyMade(float rate, bool decision)
        {
            var rateSampler = new RateSampler(rate);

            for (var i = 0; i < 1000; i++)
            {
                var traceContext = _fixture.Create<SpanContext>();
                traceContext.Debug = false;
                traceContext.Sampled = decision;

                var sampled = rateSampler.IsSampled(traceContext);

                if (decision)
                {
                    Assert.True(sampled);
                }
                else
                {
                    Assert.False(sampled);
                }
            }
        }
    }
}
