using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zipkin.NET.Core;

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

            // Register Zipkin dependencies
            services.AddZipkin("test-api", "http://localhost:8888");

            // Register ZipkinHandler for HttpClients
            services.AddHttpClient("tracingClient").AddZipkinMessageHandler("google");
            services.AddHttpClient("tracingClient2").AddZipkinMessageHandler("google-2");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Register tracing middleware
            app.UseZipkinTracing();

            app.UseMvc();
        }
    }
}
