using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zipkin.NET.Core;
using Zipkin.NET.Core.Logging;
using Zipkin.NET.Core.Reporters;
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

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Register Zipkin dependencies
            services.AddTracingMiddleware("test-api");

            // Register ZipkinHandler for HttpClients
            services.AddHttpClient("tracingClient").AddTracingMessageHandler("reqres.in-1");
            services.AddHttpClient("tracingClient2").AddTracingMessageHandler("reqres.in-2");
            services.AddHttpClient("owinClient").AddTracingMessageHandler("owin-demo");

            // Register .NET Core ILogger span reporter
            services.AddTransient<IReporter, LoggerReporter>();

            // Register Zipkin server reporter
            services.AddTransient<IReporter>(provider =>
            {
                var sender = new ZipkinHttpSender("http://localhost:9411");
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

            var reporters = app.ApplicationServices.GetServices<IReporter>();
            var sampler = new PercentageSampler(1f);

            app.UseTracer(reporters, sampler);
            app.UseTracingMiddleware();
            app.UseMvc();
        }
    }
}
