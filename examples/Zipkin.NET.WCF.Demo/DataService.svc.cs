using System;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;
using Zipkin.NET.Dispatchers;
using Zipkin.NET.Framework;
using Zipkin.NET.Logging;
using Zipkin.NET.Reporters;
using Zipkin.NET.Sampling;
using Zipkin.NET.Senders;
using Zipkin.NET.WCF.Behaviors;

namespace Zipkin.NET.WCF.Demo
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DataService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select DataService.svc or DataService.svc.cs at the Solution Explorer and start debugging.
    public class DataService : IDataService
    {
        public static void Configure(ServiceConfiguration config)
        {
            config.Description.Behaviors.Add(
                new ServiceTracingBehavior("demo-service", "http://localhost:9411", 1f));
            config.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            config.Description.Behaviors.Add(new ServiceDebugBehavior { IncludeExceptionDetailInFaults = true });
        }

        public async Task<string> GetData(int value)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost:9055/api/owin/status"));
            var httpClient = new HttpClient(new TracingHandler(
                new HttpClientHandler(),
                new SystemWebHttpContextTraceContextAccessor(), 
                new AsyncActionBlockDispatcher(new []
                {
                    new ZipkinReporter(new ZipkinHttpSender("http://localhost:9411")), 
                }, new ConsoleInstrumentationLogger()), new RateSampler(1f), "owin-api-wcf"));

            var result = await httpClient.SendAsync(httpRequest);

            return string.Format("You entered: {0}", await result.Content.ReadAsStringAsync());
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
