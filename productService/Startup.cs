
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using productService.Models;
using StackExchange.Redis;

namespace productService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //sql設定
            services.AddDbContext<ProductDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            //cors設定
            //services.AddCors(options =>
            //{
            //    options.AddDefaultPolicy(
            //        builder =>
            //        {
            //            builder.WithOrigins("*")
            //                                .AllowAnyHeader()
            //                                .AllowAnyMethod();
            //        });

            //});

            //壓縮設定
            //services.AddResponseCompression(options =>
            //{
            //    //options.EnableForHttps = true;
            //    //options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
            //    //{
            //    //    "image/png"
            //    //});

            //    options.Providers.Add<BrotliCompressionProvider>();
            //});
            //services.Configure<BrotliCompressionProviderOptions>(options =>
            //{
            //    options.Level = CompressionLevel.Optimal;
            //});

            //設定redis
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("61.218.5.103:6379"));

            //設定https
            //services.AddHttpsRedirection(options =>
            //{
            //    options.HttpsPort = 5001;
            //});

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //設定靜態資源路徑
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "imgs")),
            //    RequestPath = "/imgs"
            //});

            //proxy 
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseAuthentication();

            //app.UseHttpsRedirection();

            //app.UseCors();
            //app.UseResponseCompression();
            app.UseMvc();
        }
    }

    //ServiceEntity serviceEntity = new ServiceEntity
    //{
    //    IP = Configuration["localIP"],
    //    Port = Convert.ToInt32(Configuration["Service:Port"]),
    //    ServiceName = Configuration["Service:Name"],
    //    ConsulIP = Configuration["localIP"],
    //    ConsulPort = Convert.ToInt32(Configuration["Consul:Port"])
    //};
    //app.RegisterConsul(lifetime, serviceEntity);
    //public class ServiceEntity
    //{
    //    public string IP { get; set; }
    //    public int Port { get; set; }
    //    public string ServiceName { get; set; }
    //    public string ConsulIP { get; set; }
    //    public int ConsulPort { get; set; }
    //}

    //public static class AppBuilderExtensions
    //{
    //    public static IApplicationBuilder RegisterConsul(this IApplicationBuilder app, IApplicationLifetime lifetime, ServiceEntity serviceEntity)
    //    {
    //        var httpCheck = new AgentServiceCheck()
    //        {
    //            DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务启动多久后注册
    //            Interval = TimeSpan.FromSeconds(10),//健康检查时间间隔，或者称为心跳间隔
    //            HTTP = $"http://{serviceEntity.IP}:{serviceEntity.Port}/api/values",//健康检查地址
    //            Timeout = TimeSpan.FromSeconds(5)
    //        };

    //        // Register service with consul
    //        var registration = new AgentServiceRegistration()
    //        {
    //            Checks = new[] { httpCheck },
    //            ID = Guid.NewGuid().ToString(),
    //            Name = serviceEntity.ServiceName,
    //            Address = serviceEntity.IP,
    //            Port = serviceEntity.Port,
    //            //Tags = new[] { $"urlprefix-/{serviceEntity.ServiceName}" }//添加 urlprefix-/servicename 格式的 tag 标签，以便 Fabio 识别
    //        };

    //        var consulClient = new ConsulClient(x => x.Address = new Uri("http://" + serviceEntity.ConsulIP + ":" + serviceEntity.ConsulPort));//请求注册的 Consul 地址

    //        consulClient.Agent.ServiceRegister(registration).Wait();//服务启动时注册，内部实现其实就是使用 Consul API 进行注册（HttpClient发起）

    //        lifetime.ApplicationStopping.Register(() =>
    //        {
    //            consulClient.Agent.ServiceDeregister(registration.ID).Wait();//服务停止时取消注册
    //        });

    //        return app;
    //    }
    //}
}
