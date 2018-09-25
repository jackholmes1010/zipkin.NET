using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Zipkin.NET.Reporters;
using Zipkin.NET.Senders;

namespace Zipkin.NET.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Register zipkin reporter
            var sender = new HttpSender("http://localhost:9411");
            var reporter = new Reporter(sender);
            TraceManager.Register(reporter);

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
