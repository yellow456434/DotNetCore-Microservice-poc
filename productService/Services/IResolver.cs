using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace productService.Services
{
    public interface IServiceResolver
    {
        IService GetServiceByName(string key);
    }

    public class ServiceResolver : IServiceResolver
    {
        private readonly IServiceProvider serviceProvider;
        public ServiceResolver(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public IService GetServiceByName(string key)
        {
            switch (key)
            {
                case "A":
                    return serviceProvider.GetService<ServiceA>();

                case "B":
                    return serviceProvider.GetService<ServiceB>();
                default:
                    throw new Exception("error");
            }   
        }
    }

    public interface IService
    {
    }
    public class ServiceA : IService
    {
        public ServiceA()
        {
            System.Diagnostics.Debug.WriteLine("ServiceA");
        }
    }
    public class ServiceB : IService
    {
        public ServiceB()
        {
            System.Diagnostics.Debug.WriteLine("ServiceB");
        }
    }
}
