using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Microsoft.AspNetCore.Http;
using Zipkin.NET.Instrumentation.Reporting;

namespace Zipkin.NET.Instrumentation.WCF
{
	/// <summary>
	/// Endpoint behavior responsible for applying the <see cref="ZipkinMessageInspector"/> to the client runtime.
	/// </summary>
	/// <inheritdoc />
	public class ZipkinEndpointBehavior : IEndpointBehavior
	{
		/// <inheritdoc />
		public void Validate(ServiceEndpoint endpoint)
		{
		}

		/// <inheritdoc />
		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		/// <inheritdoc />
		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
		}

		/// <inheritdoc />
		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			var traceContextAccessor = new HttpContextTraceContextAccessor(new HttpContextAccessor());
			var reporter = new Reporter(new HttpSender("http://localhost:8888"));
			clientRuntime.ClientMessageInspectors.Add(new ZipkinMessageInspector(traceContextAccessor, reporter));
		}
	}
}
