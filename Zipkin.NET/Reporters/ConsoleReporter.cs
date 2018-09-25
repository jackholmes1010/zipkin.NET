using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Zipkin.NET.Models;

namespace Zipkin.NET.Reporters
{
    public class ConsoleReporter : IReporter
    {
        public Task ReportAsync(Span span)
        {
            var serializedSpan = JsonConvert.SerializeObject(span);
            Console.WriteLine(serializedSpan);
            return Task.CompletedTask;
        }
    }
}
