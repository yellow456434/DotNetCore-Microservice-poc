using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsulService.Services
{
    public static class ConsulBuilderExtensions
    {
        public static IApplicationBuilder RegisterConsul(this IApplicationBuilder app, IApplicationLifetime lifetime)
        {
            var consulClient = new ConsulClient(x =>
            {
                x.Address = new Uri("http://localhost:"+ Environment.GetEnvironmentVariable("consulPort"));
            });

            var registration = new AgentServiceRegistration()
            {
                ID = Guid.NewGuid().ToString(),
                Name = "ServiceA",
                Address = "localhost",
                Port = Convert.ToInt32(Environment.GetEnvironmentVariable("port")),
                Check = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30),
                    Interval = TimeSpan.FromSeconds(10),
                    HTTP = "http://host.docker.internal:" + Environment.GetEnvironmentVariable("port") + "/1",
                    Timeout = TimeSpan.FromSeconds(5)
                }
            };

            consulClient.Agent.ServiceRegister(registration).Wait();

            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });

            return app;
        }
    }
}
