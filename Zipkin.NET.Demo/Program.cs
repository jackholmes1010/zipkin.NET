using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Zipkin.NET.Logging;
using Zipkin.NET.Reporters;
using Zipkin.NET.Senders;

namespace Zipkin.NET.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
