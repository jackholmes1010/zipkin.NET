using System;

namespace Zipkin.NET.Propagation
{
    public abstract class SpanContextInjector<TInject> : ISpanContextInjector<TInject>
    {
        public TInject Inject(TInject inject, SpanContext spanContext)
        {
            if (spanContext.Sampled == null)
            {
                throw new Exception(
                    "TraceContext Sampled property is null. Make a sampling decision before propagating span context using TraceContext.Sample().");
            }

            return InjectSpanContext(inject, spanContext);
        }

        protected abstract TInject InjectSpanContext(TInject inject, SpanContext spanContext);
    }
}
