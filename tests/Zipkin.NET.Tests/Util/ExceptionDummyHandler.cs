using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Zipkin.NET.Tests.Util
{
    public class ExceptionDummyHandler : DelegatingHandler
    {
        private readonly Action<HttpRequestMessage> _requestValidator;
        private readonly Exception _exception;

        public ExceptionDummyHandler(Action<HttpRequestMessage> requestValidator, Exception exception)
        {
            _requestValidator = requestValidator;
            _exception = exception;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _requestValidator.Invoke(request);
            throw _exception;
        }
    }
}
