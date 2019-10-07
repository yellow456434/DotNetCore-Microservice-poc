using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace OrderService
{
    public class Program
    {
        public static void Main(string[] args)
        {

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
            .ConfigureKestrel((context, options) =>
                {
                    options.ListenAnyIP(5001);
                    //options.ListenAnyIP(IPAddress.Loopback, 5001, listenOptions =>
                    //{
                    //    //listenOptions.UseHttps("testCert.pfx", "testPassword");
                    //    listenOptions.UseHttps();
                    //});
                });
    }
}
