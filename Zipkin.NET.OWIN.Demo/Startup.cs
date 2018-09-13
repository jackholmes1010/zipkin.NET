using System;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;
using Microsoft.Owin.Extensions;
using Owin;
using Zipkin.NET.Instrumentation.Reporting;
using Zipkin.NET.Instrumentation.Sampling;

namespace Zipkin.NET.OWIN.Demo
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder app)
        {

            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //appBuilder.UseZipkin("owin-demo", "http://localhost:9411");
            app.Use(async (ctx, next) =>
            {
                try
                {
                    var sender = new HttpSender("http://localhost:9411");
                    var reporter = new Reporter(sender);
                    var sampler = new DebugSampler();
                    var propagator = new OwinContextB3Extractor();
                    var traceContextAccessor = new CallContextTraceContextAccessor();
                    var middleware = new ZipkinMiddleware(
                        "owin-demo", reporter, sampler, traceContextAccessor, propagator);
                    await middleware.Invoke(ctx, next);
                }
                catch (Exception ex)
                {

                }
            });

            app.UseWebApi(config);
        }
    }
}
