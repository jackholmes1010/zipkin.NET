using System.Collections.Generic;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
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
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

            var builder = new ContainerBuilder();
            builder.Register(ctx => new ZipkinHttpSender("http://localhost:9411")).As<ISender>().SingleInstance();
            builder.RegisterType<AsyncActionBlockDispatcher>().As<IDispatcher>().SingleInstance();
            builder.RegisterType<CallContextSpanContextAccessor>().As<ISpanContextAccessor>().SingleInstance();
            builder.RegisterType<ConsoleInstrumentationLogger>().As<IInstrumentationLogger>().SingleInstance();
            builder.RegisterType<RateSampler>().As<ISampler>().WithParameter("rate", 1f).SingleInstance();
            builder.RegisterType<ConsoleReporter>().As<IReporter>().SingleInstance();
            builder.RegisterType<ZipkinReporter>().As<IReporter>().SingleInstance();
            builder.RegisterType<TracingMiddleware>().WithParameter("localEndpointName", "owin-api");

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterWebApiFilterProvider(config);
            builder.RegisterWebApiModelBinderProvider();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            app.UseAutofacLifetimeScopeInjector(container);
            app.UseMiddlewareFromContainer<TracingMiddleware>();
            app.UseAutofacWebApi(config);
            app.UseWebApi(config);
        }
    }
}
