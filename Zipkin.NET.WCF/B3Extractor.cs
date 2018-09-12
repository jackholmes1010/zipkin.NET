using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Constants;
using Zipkin.NET.Instrumentation.Propagation;
using Zipkin.NET.Instrumentation.Sampling;

namespace Zipkin.NET.WCF
{
    public class B3Extractor : IExtractor<IncomingWebRequestContext>
    {
        private readonly ISampler _sampler;

        public B3Extractor(ISampler sampler)
        {
            _sampler = sampler;
        }

        public TraceContext Extract(IncomingWebRequestContext context)
        {
            var traceContext = new TraceContext(_sampler)
            {
                TraceId = context.Headers[B3HeaderConstants.TraceId],
                SpanId = context.Headers[B3HeaderConstants.SpanId],
                Debug = context.Headers[B3HeaderConstants.Flags] == "1"
            };

            return traceContext;
        }
    }
}
