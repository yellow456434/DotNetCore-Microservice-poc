using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderService.Services
{
    public class MessageProc : BackgroundService
    {
        private readonly ILogger logger;
        private IConnection connection;
        private IModel channel;

        public MessageProc(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<MessageProc>();

            var factory = new ConnectionFactory() { HostName = "localhost", Port= 5672 };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(queue: "hello",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("inininininininininininininininin");
            stoppingToken.ThrowIfCancellationRequested();            

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);

                logger.LogInformation($"consumer received {0}", message);

                channel.BasicAck(ea.DeliveryTag, false);
            };

            channel.BasicConsume(queue: "hello",
                                 autoAck: false,
                                 consumer: consumer);


            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            channel.Close();
            connection.Close();
            base.Dispose();
        }
    }
}
