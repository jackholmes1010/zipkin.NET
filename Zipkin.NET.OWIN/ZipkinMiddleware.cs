using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Models;
using Zipkin.NET.Instrumentation.Propagation;
using Zipkin.NET.Instrumentation.Reporting;
using Zipkin.NET.Instrumentation.Sampling;
using Zipkin.NET.Instrumentation.Traces;

namespace Zipkin.NET.OWIN
{
    public class ZipkinMiddleware
    {
        private readonly string _applicationName;
        private readonly IReporter _reporter;
        private readonly ISampler _sampler;
        private readonly ITraceContextAccessor _traceContextAccessor;
        private readonly IExtractor<IOwinContext> _extractor;

        public ZipkinMiddleware(
            string applicationName,
            IReporter reporter,
            ISampler sampler,
            ITraceContextAccessor traceContextAccessor,
            IExtractor<IOwinContext> extractor)
        {
            _applicationName = applicationName;
            _reporter = reporter;
            _sampler = sampler;
            _traceContextAccessor = traceContextAccessor;
            _extractor = extractor;
        }

        public async Task Invoke(IOwinContext context, Func<Task> next)
        {
            var clientTrace = _extractor
                .Extract(context)
                .NewChildTrace()
                .Sample(_sampler);

            // Record the server trace context so we can
            // later retrieve the values for the client trace.
            _traceContextAccessor.Context = clientTrace;

            var serverTrace = new ServerTrace(
                clientTrace,
                context.Request.Method,
                local: new Endpoint
                {
                    ServiceName = _applicationName
                });

            serverTrace.Tag("uri", context.Request.Path.Value);
            serverTrace.Tag("method", context.Request.Method);

            // Record server receive start time and start duration timer
            serverTrace.Start();

            try
            {
                await next();
            }
            catch (Exception ex)
            {
                serverTrace.Error(ex.Message);
                throw;
            }
            finally
            {
                serverTrace.End();

                // Report completed span to Zipkin
                _reporter.Report(serverTrace);
            }
        }
    }
}
