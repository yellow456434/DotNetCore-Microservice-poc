using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace productService.Services
{
    public class RabbitMQService : IDisposable
    {
        private readonly IConnection iconnection;

        public RabbitMQService()
        {
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };

            iconnection = factory.CreateConnection();
        }

        public IConnection getConnection()
        {
            return iconnection;
        }

        public void Dispose()
        {
            //var logFile = System.IO.File.Create(@"C:\Users\WillyCompany\Desktop\rabbitmq.txt");
            //var logWriter = new System.IO.StreamWriter(logFile);
            //logWriter.WriteLine("i'm come here");
            //logWriter.Dispose();

            iconnection.Close();
        }
    }
}
