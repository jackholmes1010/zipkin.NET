using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Models;
using Zipkin.NET.Instrumentation.Propagation;
using Zipkin.NET.Instrumentation.Reporting;
using Zipkin.NET.Instrumentation.Sampling;

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
            // Extract X-B3 headers
            var traceContext = _extractor
                .Extract(context)
	            .NewChildTrace();

            // Record the server trace context so we can
            // later retrieve the values for the client trace.
            _traceContextAccessor.Context = traceContext;

            var serverTrace = new ServerTrace(
                _sampler,
                traceContext,
                context.Request.Method,
                localEndpoint: new Endpoint
                {
                    ServiceName = _applicationName
                });

            // Record server recieve start time and start duration timer
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
