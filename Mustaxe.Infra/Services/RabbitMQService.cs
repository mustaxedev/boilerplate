using Microsoft.Extensions.Options;
using Mustaxe.Infra.Configurations.MessageBroker;
using Mustaxe.Infra.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mustaxe.Infra.Services
{
    public class RabbitMQService : IMessageBrokerService<RabbitMQMessage>
    {
        private readonly RabbitMQSettings _rabbitMQSettings;
        private IConnection _connection;

        public RabbitMQService(IOptions<RabbitMQSettings> rabbitMQSettings)
        {
            _rabbitMQSettings = rabbitMQSettings.Value;

            CreateConnection();
        }

        public void SendMessage(RabbitMQMessage message)
        {
            if (ConnectionExists()) 
            {
                using (var channel = _connection.CreateModel()) 
                {
                    channel.QueueDeclare(queue: message.Queue, durable: false, exclusive: false, autoDelete: false, arguments: null);

                    var body = Encoding.UTF8.GetBytes(message.Message);

                    channel.BasicPublish(exchange: "", routingKey: message.Queue, basicProperties: null, body: body);
                }
            }
        }

        private void CreateConnection() 
        {
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMQSettings.BaseURL,
                UserName = _rabbitMQSettings.UserName,
                Password = _rabbitMQSettings.Password,
                Port = int.Parse(_rabbitMQSettings.Port)
            };

            _connection = factory.CreateConnection();
        }

        private bool ConnectionExists() 
        {
            if (_connection is not null)
                return true;

            CreateConnection();

            return true;
        }
    }
}
