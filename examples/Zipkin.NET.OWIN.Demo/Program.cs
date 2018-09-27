using System;
using System.Collections.Generic;
using Microsoft.Owin.Hosting;
using Zipkin.NET.Framework;
using Zipkin.NET.Logging;
using Zipkin.NET.Reporters;
using Zipkin.NET.Sampling;
using Zipkin.NET.Senders;

namespace Zipkin.NET.OWIN.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Register zipkin reporter
            var sender = new ZipkinHttpSender("http://localhost:9411");
            var zipkinReporter = new ZipkinReporter(sender);
            Tracer.Start(
                new RateSampler(1f),
                new CallContextTraceContextAccessor(), 
                new ConsoleInstrumentationLogger(), 
                new List<IReporter> {zipkinReporter, new ConsoleReporter()});

            const string baseAddress = "http://localhost:9055/";

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine($"Listening on {baseAddress}...");
                Console.ReadLine();
            }
        }
    }
}
