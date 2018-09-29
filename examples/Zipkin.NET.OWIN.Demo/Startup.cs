using System.Web.Http;
using Owin;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Framework;
using Zipkin.NET.Sampling;

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
                var middleware = new TracingMiddleware(
                    "owin-api",
                    new CallContextTraceContextAccessor(), 
                    StaticDependencies.Get<Dispatcher>(),
                    new RateSampler(1f));

                await middleware.Invoke(ctx, next);
            });

            app.UseWebApi(config);
        }
    }
}
