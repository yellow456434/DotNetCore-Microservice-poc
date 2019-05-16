using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Web;
using productService.Models;

namespace productService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

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

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseNLog();

        public static void Initialize(ProductDbContext productDbContext)
        {
            var products = new Product[]
            {
                new Product(){ Name = "P1", Price = 111, CreatedTime = DateTime.Now },
                new Product(){ Name = "P2", Price = 222, CreatedTime = DateTime.Now, RemovedTime = DateTime.Now.AddMinutes(5) }
            };

            foreach (var p in products)
            {
                productDbContext.Products.Add(p);
            }

            productDbContext.SaveChanges();
        }
    }
}
