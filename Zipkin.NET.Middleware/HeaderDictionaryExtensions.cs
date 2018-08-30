using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Zipkin.NET.Instrumentation
{
	public static class HeaderDictionaryExtensions
	{
		public static TraceContext ExtractTraceContext(this IHeaderDictionary dictionary)
		{
			var traceContext = new TraceContext();

			if (dictionary.TryGetValue(B3HeaderConstants.TraceId, out var value))
			{
				traceContext.TraceId = value;
			}

			if (dictionary.TryGetValue(B3HeaderConstants.SpanId, out value))
			{
				traceContext.SpanId = value;
			}

			return traceContext;
		}
	}
}
