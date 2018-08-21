using System;
using System.Text;

namespace Zipkin.NET.Instrumentation
{
	public class TraceIdentifierGenerator : ITraceIdentifierGenerator
	{
		public string GenerateId()
		{
			// TODO this is stupid
			var random = new Random();
			var builder = new StringBuilder();
			for (var i = 0; i < 16; i++)
			{
				builder.Append(random.Next(0, 15).ToString("X").ToLower());
			}

			return builder.ToString();
		}
	}
}
