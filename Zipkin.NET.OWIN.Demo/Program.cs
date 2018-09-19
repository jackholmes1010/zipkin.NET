using System;
using System.Net.Http;
using Microsoft.Owin.Hosting;

namespace Zipkin.NET.OWIN.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
