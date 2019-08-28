using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Web;
using productService.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace productService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            if (isService)
            {
                //serviceRoot
                Directory.SetCurrentDirectory(@"D:\Web\BackEndApi");
            }

            var builder = CreateWebHostBuilder(
                args.Where(arg => arg != "--console").ToArray());

            var host = builder.Build();

            #region 初始化DB資料
            //using (var scope = host.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    try
            //    {
            //        var context = services.GetRequiredService<Models.ProductDbContext>();
            //        //Initialize(context);
            //    }
            //    catch (Exception ex)
            //    {
            //        var logger = services.GetRequiredService<ILogger<Program>>();
            //        logger.LogError(ex, "An error occurred while seeding the database.");
            //    }
            //}
            #endregion

            if (isService)
            {
                host.RunAsService();
            }
            else
            {
                host.Run();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) => config.AddEnvironmentVariables())
                .UseStartup<Startup>()
                .ConfigureKestrel((context, options) =>
                {
                    options.ListenAnyIP(5000);
                    //options.ListenAnyIP(IPAddress.Loopback, 5001, listenOptions =>
                    //{
                    //    //listenOptions.UseHttps("testCert.pfx", "testPassword");
                    //    listenOptions.UseHttps();
                    //});
                })
                .UseNLog();

        public static void Initialize(ProductDbContext productDbContext)
        {
            var products = new Product[]
            {
                new Product(){ Name = "P1", Price = 111, /*CreatedTime = DateTime.Now*/ },
                new Product(){ Name = "P2", Price = 222, /*CreatedTime = DateTime.Now,*/ RemovedTime = DateTime.Now.AddMinutes(5) }
            };

            foreach (var p in products)
            {
                productDbContext.Products.Add(p);
            }

            productDbContext.SaveChanges();
        }
    }
}
