
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using productService.Models;
using productService.Services;
using StackExchange.Redis;

namespace productService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //sql設定
            services.AddDbContext<ProductDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //identity Core
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });

            //cookies
            //services.ConfigureApplicationCookie(options =>
            //{
            //    options.Events = new CookieAuthenticationEvents
            //    {
            //        OnRedirectToLogin = ctx =>
            //        {
            //            if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
            //            {
            //                ctx.Response.StatusCode = 401;
            //                return Task.FromResult<object>(null);
            //            }

            //            ctx.Response.Redirect(ctx.RedirectUri);
            //            return Task.FromResult<object>(null);
            //        }
            //    };
            //    // Cookie settings
            //    options.Cookie.HttpOnly = true;
            //    options.ExpireTimeSpan = TimeSpan.FromSeconds(5);

            //    //options.LoginPath = "/Identity/Account/Login";
            //    //options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            //    //options.SlidingExpiration = true;
            //});

            //cors設定
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("*")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                    });
            });

            //jwt
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                })
                .AddJwtBearer(jwtBearerOptions =>
                {
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        //ValidIssuer = "123",
                        //ValidAudience = "123",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["jwtKey"]))
                    };
                });

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

            //特定導向https 5001
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

            //授權
            app.UseAuthentication();

            //特定導向https
            //app.UseHttpsRedirection();

            app.UseCors();
            //app.UseResponseCompression();

            app.Map("/api/midd", mapApp =>
            {
                
                mapApp.Use(async (context, next) =>
                {
                    await context.Response.WriteAsync("1 Middleware in. \r\n");
                    await next.Invoke();
                   //await context.Response.WriteAsync("Second Middleware out. \r\n");
                });
                mapApp.UseMiddleware<CustomTestMiddleware>();
            });

            app.UseMvc();
        }
    }

    /*
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
    */
}
