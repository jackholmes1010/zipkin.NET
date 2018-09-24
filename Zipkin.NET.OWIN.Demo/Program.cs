﻿using System;
using Microsoft.Owin.Hosting;
using Zipkin.NET.Reporters;
using Zipkin.NET.Senders;

namespace Zipkin.NET.OWIN.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Register zipkin reporter
            var sender = new HttpSender("http://localhost:8888");
            var reporter = new Reporter(sender);
            TraceManager.Register(reporter);

            string baseAddress = "http://localhost:9055/";

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine($"Listening on {baseAddress}...");
                Console.ReadLine();
            }
        }
    }
}
