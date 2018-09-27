using System;
using System.Collections.Generic;
using System.Text;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;

namespace Zipkin.NET
{
    public class ServerTrace<TRequest>
    {
        private readonly string _name;
        private readonly IExtractor<TRequest> _extractor;
        private readonly TRequest _request;

        private TraceContext _traceContext;
        private SpanBuilder _spanBuilder;

        public ServerTrace(string name, IExtractor<TRequest> extractor, TRequest request)
        {
            _name = name;
            _extractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
            _request = request;
        }

        public TResult Start<TResult>(string name, Func<SpanBuilder, TResult> work)
        {

            try
            {
                _traceContext = _extractor.Extract(_request);
                _spanBuilder = _traceContext.GetSpanBuilder();

                Tracer.Sampler.Sample(ref _traceContext);

                _spanBuilder
                    .Start()
                    .Kind(SpanKind.Server)
                    .WithLocalEndpoint(new Endpoint
                    {
                        ServiceName = _name
                    });

                var result = work(_spanBuilder);

                Tracer.ContextAccessor.SaveTrace(_traceContext);

                return result;
            }
            catch (Exception ex)
            {
                _spanBuilder.Error(ex.Message);
                throw;
            }
            finally
            {
                Tracer.Report(_traceContext, _spanBuilder.Build());
            }
        }
    }
}
