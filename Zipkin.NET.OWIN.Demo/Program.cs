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
                // Create HttpCient and make a request to api/values 
                //HttpClient client = new HttpClient();

                //var response = client.GetAsync(baseAddress + "api/owin/status").Result;

                //Console.WriteLine(response);
                //Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                //Console.ReadLine();
                Console.WriteLine($"Listening on {baseAddress}...");
                Console.ReadLine();
            }
        }
    }
}
