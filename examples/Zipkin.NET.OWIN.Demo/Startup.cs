using System.Collections.Generic;
using System.Web.Http;
using Owin;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Framework;
using Zipkin.NET.Logging;
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
                // Register zipkin reporter
                var sender = new ZipkinHttpSender("http://localhost:9411");
                var zipkinReporter = new ZipkinReporter(sender); var logger = new ConsoleInstrumentationLogger();
                var reporters = new List<IReporter> { zipkinReporter, new ConsoleReporter() };
                var dispatcher = new AsyncActionBlockDispatcher(reporters, logger);

                var middleware = new TracingMiddleware(
                    "owin-api",
                    new CallContextTraceContextAccessor(),
                    dispatcher,
                    new RateSampler(1f));

                await middleware.Invoke(ctx, next);
            });

            app.UseWebApi(config);
        }
    }
}
