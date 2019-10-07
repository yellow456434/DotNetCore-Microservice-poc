using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace productService.Services
{
    public class RpcClient
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string replyQueueName;
        private readonly EventingBasicConsumer consumer;
        //private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
        private BlockingCollection<string> msg = new BlockingCollection<string>();
        private readonly BlockingCollection<AutoResetEvent> waitList = new BlockingCollection<AutoResetEvent>();
        //private string msg = "";
        //private readonly AutoResetEvent wait = new AutoResetEvent(false);
        private readonly IBasicProperties props;
        private readonly string correlationId;

        public RpcClient()
        {
            Console.WriteLine("RpcClient init");
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            props = channel.CreateBasicProperties();

            correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;


            replyQueueName = channel.QueueDeclare("test1", false, false, false).QueueName;
            props.ReplyTo = replyQueueName;

            consumer = new EventingBasicConsumer(channel);
            consumer.Received += ReplyQueue_Received;

            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: false);
        }

        public async Task<string> Call(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(
                exchange: "",
                routingKey: "rpc_queue",
                basicProperties: props,
                body: messageBytes);

            AutoResetEvent wait = new AutoResetEvent(false);
            waitList.Add(wait);

            await Task.Run(() => { wait.WaitOne(); });

            var m = msg.Take();
            //var m = msg;

            return m;
        }

        public void Close()
        {
            consumer.Received -= ReplyQueue_Received;
            channel.Dispose();
            connection.Close();
        }

        private void ReplyQueue_Received(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var response = Encoding.UTF8.GetString(body);

            if (ea.BasicProperties.CorrelationId == correlationId)
            {
                msg.Add(response);
                waitList.Take().Set();

                //msg = response;
                //wait.Set();

                channel.BasicAck(ea.DeliveryTag, false);

                this.Close();
            }

        }
    }
}
