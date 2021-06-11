﻿using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Core.Utilities.MessageBrokers.RabbitMq
{
    public class MqConsumerHelper : IMessageConsumer
    {
        private readonly MessageBrokerOptions _brokerOptions;

        public MqConsumerHelper(IConfiguration configuration)
        {
            _brokerOptions = configuration.GetSection("MessageBrokerOptions").Get<MessageBrokerOptions>();
        }

        public void GetQueue()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _brokerOptions.HostName,
                UserName = _brokerOptions.UserName,
                Password = _brokerOptions.Password
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "DArchQueue",
                                                     durable: true,
                                                     exclusive: false,
                                                     autoDelete: false,
                                                     arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, mq) =>
            {
                var body = mq.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"Message: {message}");
            };

            channel.BasicConsume(queue: "DArchQueue",
                                                  autoAck: false,
                                                  consumer: consumer);
            Console.ReadKey();

        }
    }
}