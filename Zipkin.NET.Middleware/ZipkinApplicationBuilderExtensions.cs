using System;
using Microsoft.AspNetCore.Builder;

namespace Zipkin.NET.Middleware
{
	public static class ZipkinApplicationBuilderExtensions
	{
		public static IApplicationBuilder UseZipkinTracing(this IApplicationBuilder app)
		{
			if (app == null)
				throw new ArgumentNullException(nameof(app));

			return app.UseMiddleware<ZipkinMiddleware>();
		}
	}
}
