using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Constants;
using Zipkin.NET.Instrumentation.Models;
using Zipkin.NET.Instrumentation.Propagation;
using Zipkin.NET.Instrumentation.Reporting;
using Zipkin.NET.Instrumentation.Sampling;

namespace Zipkin.NET.Clients.WCF
{
    /// <summary>
    /// Message inspector responsible for adding Zipkin X-B3 HTTP headers to outgoing WCF requests.
    /// </summary>
    /// <inheritdoc />
    public class ZipkinMessageInspector : IClientMessageInspector
    {
        private readonly string _applicationName;
        private readonly IReporter _reporter;
        private readonly ISampler _sampler;
        private readonly ITraceContextAccessor _traceContextAccessor;
        private readonly IPropagator<HttpRequestMessageProperty> _propagator;

        private ClientTrace _trace;

        public ZipkinMessageInspector(
            string applicationName,
            IReporter reporter,
            ISampler sampler,
            ITraceContextAccessor traceContextAccessor,
            IPropagator<HttpRequestMessageProperty> propagator)
        {
            _applicationName = applicationName;
            _reporter = reporter;
            _sampler = sampler;
            _traceContextAccessor = traceContextAccessor;
            _propagator = propagator;
        }

        /// <inheritdoc />
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var traceContext = _traceContextAccessor.Context;

            var httpRequest = ExtractHttpRequest(request);

            // Inject X-B3 headers to the outgoing request
            _propagator.Inject(httpRequest, traceContext);

            _trace = new ClientTrace(
                _sampler,
                traceContext,
                request.Headers.Action,
                remoteEndpoint: new Endpoint
                {
                    ServiceName = _applicationName
                });

            _trace.NewChild();

            _trace.Start();

            return null;
        }

        /// <inheritdoc />
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            _trace.End();
            _reporter.Report(_trace);
        }

        private static HttpRequestMessageProperty ExtractHttpRequest(Message wcfMessage)
        {
            HttpRequestMessageProperty httpRequest;
            if (wcfMessage.Properties.TryGetValue(HttpRequestMessageProperty.Name, out var requestObject))
            {
                httpRequest = requestObject as HttpRequestMessageProperty;
            }
            else
            {
                httpRequest = new HttpRequestMessageProperty();
                wcfMessage.Properties.Add(HttpRequestMessageProperty.Name, httpRequest);
            }

            return httpRequest;
        }
    }
}
