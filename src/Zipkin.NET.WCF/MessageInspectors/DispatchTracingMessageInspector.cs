using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Exceptions;
using Zipkin.NET.Models;
using Zipkin.NET.Propagation;
using Zipkin.NET.Sampling;
using Zipkin.NET.WCF.Propagation;

namespace Zipkin.NET.WCF.MessageInspectors
{
    public class DispatchTracingMessageInspector :  IDispatchMessageInspector
    {
        private readonly string _localEndpointName;
        private readonly ISampler _sampler;
        private readonly IDispatcher _dispatcher;
        private readonly ISpanContextAccessor _spanContextAccessor;
        private readonly ISpanContextExtractor<IncomingWebRequestContext> _spanContextExtractor;

        public DispatchTracingMessageInspector(
            string localEndpointName,
            ISampler sampler,
            IDispatcher dispatcher,
            ISpanContextAccessor spanContextAccessor)
        {
            _localEndpointName = localEndpointName;
            _sampler = sampler;
            _dispatcher = dispatcher;
            _spanContextAccessor = spanContextAccessor;
            _spanContextExtractor = new IncomingWebRequestB3SpanContextExtractor();
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var spanContext = _spanContextExtractor
                .Extract(WebOperationContext.Current?.IncomingRequest);

            spanContext.Sample(_sampler);

            var spanBuilder = new SpanBuilder(spanContext);
            spanBuilder.Start()
                .Tag("action", request.Headers.Action)
                .Kind(SpanKind.Server)
                .WithLocalEndpoint(new Endpoint
                {
                    ServiceName = _localEndpointName
                });
                
            _spanContextAccessor.SaveContext(spanContext);

            return spanBuilder;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            var spanBuilder = (SpanBuilder) correlationState;
            var span = spanBuilder
                .End()
                .Build();

            try
            {
                _dispatcher.Dispatch(span);
            }
            catch (DispatchException)
            {
                // ignore
            }
        }
    }
}
