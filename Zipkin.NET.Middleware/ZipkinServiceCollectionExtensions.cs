using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Zipkin.NET.Instrumentation;
using Zipkin.NET.Instrumentation.Reporting;

namespace Zipkin.NET.Middleware
{
    public static class ZipkinServiceCollectionExtensions
    {
        public static IServiceCollection AddZipkin(this IServiceCollection services, string applicationName)
        {
            // TODO is this needed?
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
	        services.AddSingleton<ITraceIdentifierGenerator, TraceIdentifierGenerator>();
			services.AddTransient<ITraceContext, TraceContext>();
	        services.AddTransient<IReporter, Reporter>();
	        services.AddTransient<ISender, HttpSender>();

			// Register middleware
	        services.AddTransient(provider =>
	        {
		        var reporter = provider.GetService<IReporter>();
		        var traceContext = provider.GetService<ITraceContext>();
		        var traceIdGenerator = provider.GetService<ITraceIdentifierGenerator>();
				var middleware = new ZipkinMiddleware(applicationName, reporter, traceContext, traceIdGenerator);
		        return middleware;
	        });

            return services;
        }
    }
}
