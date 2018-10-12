using System.Collections.Generic;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Zipkin.NET.Encoding;
using Zipkin.NET.Models;

namespace Zipkin.NET.Senders.RabbitMq
{
    public class ZipkinRabbitMqSender : ISender
    {
        private readonly IEncoder _encoder;
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;

        public ZipkinRabbitMqSender(
            string hostName = "localhost", 
            int port = 5672, 
            string username = "guest", 
            string password = "guest", 
            string virtualHost = "/")
        {
            _encoder = new JsonEncoder();
            _connectionFactory = new ConnectionFactory
            {
                HostName = hostName,
                Port = port,
                UserName = username,
                Password = password, 
                VirtualHost = virtualHost
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
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var body = _encoder.Encode(spans);

                channel.BasicPublish(exchange: "",
                    routingKey: "zipkin",
                    basicProperties: null,
                    body: body);

                channel.Close();
            }

            return Task.CompletedTask;
        }
    }
}
