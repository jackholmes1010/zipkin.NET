using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Zipkin.NET.Tests.Util
{
    [ExcludeFromCodeCoverage]
    public class DummyHandler : DelegatingHandler
    {
        private readonly Action<HttpRequestMessage> _requestValidator;

        public DummyHandler(Action<HttpRequestMessage> requestValidator)
        {
            _requestValidator = requestValidator;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _requestValidator.Invoke(request);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
