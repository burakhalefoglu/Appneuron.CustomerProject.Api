﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Core.Utilities.MessageBrokers.RabbitMq
{
    public class MqQueueHelper : IMessageBrokerHelper
    {
        private readonly MessageBrokerOptions _brokerOptions;

        public MqQueueHelper(IConfiguration configuration)
        {
            _brokerOptions = configuration.GetSection("MessageBrokerOptions").Get<MessageBrokerOptions>();
        }

        public void QueueMessage(object objMessage)
        {
            var factory = new ConnectionFactory
            {
                HostName = _brokerOptions.HostName,
                UserName = _brokerOptions.UserName,
                Password = _brokerOptions.Password
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                        queue: "DArchQueue",
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);


                var message = JsonConvert.SerializeObject(objMessage);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "", routingKey: "DArchQueue", basicProperties: null, body: body);
            }
        }
    }
}