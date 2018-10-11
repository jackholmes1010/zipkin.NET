using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;
using Zipkin.NET.Models;

namespace Zipkin.NET.Senders.RabbitMq
{
    public class ZipkinRabbitMqSender : ISender
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;

        public ZipkinRabbitMqSender(string hostName)
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = hostName
            };
        }


        private IConnection Connection
        {
            get
            {
                if (_connection != null && _connection.IsOpen)
                {
                    return _connection;
                }

                _connection = _connectionFactory.CreateConnection();
                return _connection;
            }
        }

        public Task SendSpansAsync(IEnumerable<Span> spans)
        {
            using (var channel = Connection.CreateModel())
            {
                channel.QueueDeclare(queue: "spans",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var message = JsonConvert.SerializeObject(
                    spans, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                    routingKey: "zipkin",
                    basicProperties: null,
                    body: body);
            }

            return Task.CompletedTask;
        }
    }
}
