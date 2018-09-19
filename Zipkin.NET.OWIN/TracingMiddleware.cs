using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;
using Zipkin.NET.Reporters;

namespace Zipkin.NET.OWIN
{
    public class TracingMiddleware
    {
        private readonly string _applicationName;
        private readonly IReporter _reporter;
        private readonly ITraceAccessor _traceAccessor;
        private readonly IExtractor<IOwinContext> _extractor;

        public TracingMiddleware(
            string applicationName,
            IReporter reporter,
            ITraceAccessor traceAccessor,
            IExtractor<IOwinContext> extractor)
        {
            _applicationName = applicationName;
            _reporter = reporter;
            _traceAccessor = traceAccessor;
            _extractor = extractor;
        }

        public async Task Invoke(IOwinContext context, Func<Task> next)
        {
            var parentSpan = _extractor.Extract(context);
            var trace = new Trace(parentSpan.TraceId, parentSpan.Id);
            var spanBuilder = trace.GetSpanBuilder();

            spanBuilder
                .Tag("host", context.Request.Host.Value)
                .Tag("resource", context.Request.Path.Value)
                .Tag("method", context.Request.Method)
                .WithLocalEndpoint(new Endpoint
                {
                    ServiceName = _applicationName
                })
                .Start();

            _traceAccessor.SaveTrace(trace);

            try
            {
                await next();
            }
            catch (Exception ex)
            {
                spanBuilder.Error(ex.Message);
            }
            finally
            {
                spanBuilder.End();
                _reporter.Report(spanBuilder.Build());
            }
        }
    }
}
