using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace gRPC
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            Console.WriteLine(request.Birthday2.ToDateTime());

            var tt = request.Birthday2.ToDateTime().AddDays(1);

            System.Threading.Thread.Sleep(5000);

            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name,
                Birthday2 = Timestamp.FromDateTime(tt.ToUniversalTime())
            });
        }
    }
}
