using System.Web.Http;
using Owin;
using Zipkin.NET.Framework;
using Zipkin.NET.Reporters;
using Zipkin.NET.Sampling;
using Zipkin.NET.Senders;

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

            app.Use(async (ctx, next) =>
            {
                var sender = new HttpSender("http://localhost:9411");
                var reporter = new Reporter(sender);
                var propagator = new OwinContextB3Extractor();
                var traceContextAccessor = new CallContextTraceAccessor();
                var sampler = new DebugSampler();
                var middleware = new TracingMiddleware(
                    "owin-api", reporter, sampler, traceContextAccessor, propagator);
                await middleware.Invoke(ctx, next);
            });

            app.UseWebApi(config);
        }
    }
}
