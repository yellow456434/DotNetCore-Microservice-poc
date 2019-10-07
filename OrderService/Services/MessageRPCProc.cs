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
    public class MessageRPCProc : BackgroundService
    {
        private readonly ILogger logger;
        private IConnection connection;
        private IModel channel;

        public MessageRPCProc(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<MessageProc>();

            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(queue: "rpc_queue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            channel.BasicQos(0, 1, false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("inininininininininininininininin");
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: "rpc_queue",
                                 autoAck: false,
                                 consumer: consumer);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var props = ea.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                //request 參數body
                var message = Encoding.UTF8.GetString(body);


                var responseBytes = Encoding.UTF8.GetBytes(message + "response");

                //var logFile = System.IO.File.Create("/Users/willymbp/Desktop/bbb.txt");
                //var logWriter = new System.IO.StreamWriter(logFile);
                //logWriter.WriteLine(responseBytes);
                //logWriter.Dispose();

                channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                  basicProperties: replyProps, body: responseBytes);

                channel.BasicAck(deliveryTag: ea.DeliveryTag,
                  multiple: false);
            };


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
