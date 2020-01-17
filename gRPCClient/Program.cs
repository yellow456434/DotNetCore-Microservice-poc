using System;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using gRPC;
using Grpc.Net.Client;

namespace gRPCClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Send Rpc");
            AppContext.SetSwitch(
    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            using var channel = GrpcChannel.ForAddress("http://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(
                              new HelloRequest { Name = "GreeterClient", Birthday2 = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime()) });
            Console.WriteLine("Greeting: " + reply.Message); 
            Console.WriteLine("Bitthdat: " + reply.Birthday2.ToDateTime());
            Console.WriteLine("exit...");
            //Console.ReadKey();
        }
    }
}
