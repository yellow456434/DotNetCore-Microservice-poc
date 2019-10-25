using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Winton.Extensions.Configuration.Consul;

namespace ConsulService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            CreateWebHostBuilder(args, cancellationTokenSource).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args, CancellationTokenSource cts) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((hostContext, config) =>
                    config.AddConsul(
                            "test/appsettings.json",
                            cts.Token,
                            options =>
                            {
                                options.ConsulConfigurationOptions =
                                    cco => { cco.Address = new Uri("http://localhost:8500"); };
                                options.Optional = true;
                                options.ReloadOnChange = true;
                                options.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; };
                            })
                )
                .ConfigureKestrel((context, options) =>
                {
                    Environment.SetEnvironmentVariable("port", args[0]);
                    Environment.SetEnvironmentVariable("consulPort", args[1]);
                    options.ListenAnyIP(Convert.ToInt32(Environment.GetEnvironmentVariable("port")));
                });
    }
}
