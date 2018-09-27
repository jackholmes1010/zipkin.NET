using System;
using System.Collections;

namespace Zipkin.NET.Sampling
{
    /// <summary>
    /// Sampler which samples a certain percentage of traces.
    /// </summary>
    public class RateSampler : Sampler
    {
        private int _counter;
        private readonly BitArray _bitArray;
        private readonly bool _neverSample;
        private readonly bool _alwaysSample;

        /// <summary>
        /// Construct a new <see cref="RateSampler"/>.
        /// </summary>
        /// <param name="rate">
        /// The percentage of traces to sample. 1.0 means sample 
        /// everything (100%) and 0.0 means sample nothing (0%).
        /// </param>
        public RateSampler(float rate)
        {
            if (rate > 1f)
            {
                _alwaysSample = true;
            }
            else if (Math.Abs(rate) < 0.0000001f)
            {
                _neverSample = true;
            }
            else
            {
                var size = (int)(100 / (rate * 100));
                _bitArray = new BitArray(size, false);
                SetRandomBit();
            }
        }

        protected override bool MakeSamplingDecision(TraceContext traceContext)
        {
            return _alwaysSample || !_neverSample && IsSampled();
        }

        private bool IsSampled()
        {
            var result = _bitArray[_counter];

            _counter++;

            if (_counter == _bitArray.Length)
            {
                _counter = 0;
            }

            return result;
        }

        private void SetRandomBit()
        {
            var index = new Random().Next(0, _bitArray.Length);
            _bitArray[index] = true;
        }
    }
}
