using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Zipkin.NET.Instrumentation
{
	public class TraceContextAccessor : ITraceContextAccessor
	{
		private TraceContext _traceContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public TraceContextAccessor(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public TraceContext Context
		{
			//get => _traceContext ??
			//       (_traceContext = _httpContextAccessor.HttpContext.Items["server-trace"] as TraceContext);
			get => _httpContextAccessor.HttpContext.Items["server-trace"] as TraceContext;
			set
			{
				//_traceContext = value;
				_httpContextAccessor.HttpContext.Items["server-trace"] = value;
			}
		}
	}
}
