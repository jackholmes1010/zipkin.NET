using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zipkin.NET.Logging;
using Zipkin.NET.Middleware;
using Zipkin.NET.Middleware.Logging;
using Zipkin.NET.Middleware.Reporters;
using Zipkin.NET.Middleware.TraceAccessors;
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

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Register Zipkin dependencies
            services.AddZipkin("test-api");

            // Register ZipkinHandler for HttpClients
            services.AddHttpClient("tracingClient").AddZipkinMessageHandler("reqres.in-1");
            services.AddHttpClient("tracingClient2").AddZipkinMessageHandler("reqres.in-2");
            services.AddHttpClient("owinClient").AddZipkinMessageHandler("owin-demo");

            // Register .NET Core ILogger span reporter
            services.AddTransient<IReporter, LoggerReporter>();

            // Register Zipkin server reporter
            services.AddTransient<IReporter>(provider =>
            {
                var sender = new HttpSender("http://localhost:9411");
                var reporter = new ZipkinReporter(sender);
                return reporter;
            });

            // Register .NET Core ILogger tracing logger (used for exception logging)
            services.AddTransient<IInstrumentationLogger, CoreInstrumentationLogger>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseZipkin(app.ApplicationServices.GetServices<IReporter>());
            app.UseTracingMiddleware();
            app.UseMvc();
        }
    }
}
