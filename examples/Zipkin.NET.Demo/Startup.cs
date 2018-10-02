using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zipkin.NET.Core;
using Zipkin.NET.Core.Reporters;
using Zipkin.NET.Reporters;
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

            // Add a tracing handler to the HTTP clients.
            // The TracingHandler builds spans from outgoing client
            // requests and reports them using the registered reporters.
            services.AddHttpClient("tracingClient").AddTracingMessageHandler("reqres.in-1");
            services.AddHttpClient("tracingClient2").AddTracingMessageHandler("reqres.in-2");
            services.AddHttpClient("owinClient").AddTracingMessageHandler("owin-demo");

            // Register Zipkin dependencies.
            AddZipkinServices(services);
        }

        private static void AddZipkinServices(IServiceCollection services)
        {

            // Register Zipkin server reporter.
            // This reporter sends completed spans to a Zipkin 
            // server's HTTP collector (POST api/v2/spans).
            services.TryAddTransient<IReporter>(provider =>
            {
                var sender = new ZipkinHttpSender("http://localhost:9411");
                var reporter = new ZipkinReporter(sender);
                return reporter;
            });

            // Register .NET Core ILogger span reporter.
            // This reporter logs completed spans using the .NET Core ILogger.
            services.TryAddTransient<IReporter, LoggerReporter>();

            // Register default tracing dependencies.
            services.AddTracing("test-api");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Add the tracing middleware to the request pipeline.
            app.UseTracing();

            app.UseMvc();
        }
    }
}
