using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zipkin.NET.Core;
using Zipkin.NET.Core.Logging;
using Zipkin.NET.Core.Reporters;
using Zipkin.NET.Core.TraceAccessors;
using Zipkin.NET.Logging;
using Zipkin.NET.Reporters;
using Zipkin.NET.Sampling;
using Zipkin.NET.Senders;

namespace Zipkin.NET.Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddLogging();

            // Register ZipkinHandler for HttpClients.
            services.AddHttpClient("tracingClient").AddTracingMessageHandler("reqres.in-1");
            services.AddHttpClient("tracingClient2").AddTracingMessageHandler("reqres.in-2");
            services.AddHttpClient("owinClient").AddTracingMessageHandler("owin-demo");

            // Register Zipkin dependencies.
            AddZipkinServices(services);
        }

        private void AddZipkinServices(IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Register rate sampler.
            // This RateSampler will sample 100% of traces providing a
            // sampling decision has not already been made by an upstream service.
            services.AddTransient<Sampler>(provider => new RateSampler(1f));

            // Register the trace context accessor.
            // This ITraceContextAccessor will store the trace context in the HTTP context.
            // This allows middleware to store trace context (trace ID, server span ID, debug flag, and sampled 
            // flag) for use by the tracing handler (HTTP client delegating handler) for creating client spans.
            services.AddTransient<ITraceContextAccessor, HttpContextTraceContextAccessor>();

            // Register tracing middleware.
            // This middleware builds spans from incoming requests 
            // and reports them to the registered IReporters.
            services.AddTransient(provider => new TracingMiddleware("test-api"));

            // Register .NET Core ILogger span reporter.
            // This reporter logs completed spans using the .NET Core ILogger.
            services.AddTransient<IReporter, LoggerReporter>();

            // Register Zipkin server reporter.
            // This reporter sends completed spans to a Zipkin 
            // server's HTTP collector (POST api/v2/spans).
            services.AddTransient<IReporter>(provider =>
            {
                var sender = new ZipkinHttpSender("http://localhost:9411");
                var reporter = new ZipkinReporter(sender);
                return reporter;
            });

            // Register .NET Core ILogger tracing logger (used for exception logging).
            // This logger logs instrumentation exceptions using the .NET Core ILogger.
            services.AddTransient<IInstrumentationLogger, CoreInstrumentationLogger>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseTracer();
            app.UseTracingMiddleware();
            app.UseMvc();
        }
    }
}
