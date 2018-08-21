using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Zipkin.NET.Instrumentation.Reporting;

namespace Zipkin.NET.Instrumentation
{
    public static class ZipkinServiceCollectionExtensions
    {
        public static IServiceCollection AddZipkin(this IServiceCollection services)
        {
            // TODO is this needed?
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ITraceContext, TraceContext>();
	        services.AddTransient<IReporter, Reporter>();
	        services.AddTransient<ISender, HttpSender>();
	        services.AddSingleton<ITraceIdentifierGenerator, TraceIdentifierGenerator>();
            return services;
        }
    }
}
