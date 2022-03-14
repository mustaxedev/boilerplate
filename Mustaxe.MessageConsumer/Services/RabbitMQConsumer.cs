using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Mustaxe.Infra.Configurations.MessageBroker;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mustaxe.MessageConsumer.Services
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly RabbitMQSettings _rabbitMQSettings;
        private readonly ILogger _logger;

        private IConnection _connection;
        private IModel _channel;
        private readonly string _queueName = "CONSULTA_CEP_QUEUE";

        private EventHandler<ShutdownEventArgs> OnConsumerShutdown;
        private EventHandler<ConsumerEventArgs> OnConsumerRegistered;
        private EventHandler<ConsumerEventArgs> OnConsumerUnregistered;
        private EventHandler<ConsumerEventArgs> OnConsumerCancelled;

        public RabbitMQConsumer(IOptions<RabbitMQSettings> rabbitMQSettings,
            ILogger logger)
        {
            _rabbitMQSettings = rabbitMQSettings.Value;
            _logger = logger;

            CreateConnection();
            InitializeEventHandlers();
        }

        

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (ConnectionExists()) 
            {
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (ch, ea) =>
                {
                    var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var message = JsonConvert.DeserializeObject<RabbitMQMessage>(content);

                    HandleMessage(message);

                    _channel.BasicAck(ea.DeliveryTag, false);
                };
                consumer.Shutdown += OnConsumerShutdown;
                consumer.Registered += OnConsumerRegistered;
                consumer.Unregistered += OnConsumerUnregistered;
                consumer.ConsumerCancelled += OnConsumerCancelled;

                _channel.BasicConsume(_queueName, false, consumer);

                return Task.CompletedTask;
            }

            return null;
        }

        public void HandleMessage(RabbitMQMessage message) 
        {
            if (message is not null) 
            {
                _logger.Information("Mensagem consumida da fila {queue_name} com SUCESSO!", _queueName);
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
            //_connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        private void InitializeEventHandlers() 
        {
            OnConsumerShutdown = (ch, ea) =>
            {
                _logger.Information("Consumer desconectado da fila {queue_name}!", _queueName);
            };

            OnConsumerRegistered = (ch, ea) =>
            {
                _logger.Information("Consumer registrado na fila {queue_name}!", _queueName);
            };

            OnConsumerUnregistered = (ch, ea) =>
            {
                _logger.Information("Consumer desconectado da fila {queue_name}!", _queueName);
            };

            OnConsumerCancelled = (ch, ea) =>
            {
                _logger.Information("Consumer cacnelado da fila {queue_name}!", _queueName);
            };
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
