using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;

namespace Zipkin.NET.OWIN
{
    public static class TracingAppBuilderExtensions
    {
        public static IAppBuilder UseTracing(this IAppBuilder app, string localEndpointName)
        {

            return app;
        }
    }
}
