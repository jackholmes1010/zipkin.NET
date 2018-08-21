using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Zipkin.NET.Instrumentation.Models;

namespace Zipkin.NET.Instrumentation.Reporting
{
    public class Reporter : IReporter
    {
        private readonly ISender _sender;

        public Reporter(ISender sender)
        {
            _sender = sender;
        }

        public async Task ReportAsync(Span span)
        {
            // TODO this is a test
            var jsonSpan = JsonConvert.ToString(span);
            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, jsonSpan);
                var encodedSpan = memoryStream.ToArray();
                await _sender.SendSpansAsync(new List<byte[]> {encodedSpan});
            }
        }
    }
}
