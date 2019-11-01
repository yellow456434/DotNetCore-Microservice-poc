using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using productService.Middlewares;
using productService.Models;
using productService.Services;
using StackExchange.Redis;
using System;
using System.IO;
using System.Text;

namespace productService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // [Obsolete]
        //public static readonly LoggerFactory DbLoggerFactory = new LoggerFactory(new ILoggerProvider[] {
        //    new ConsoleLoggerProvider(
        //        (category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Debug,
        //        true),
        //    new NLogLoggerProvider() });

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //sql設定
            //services.AddDbContext<ProductDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            ////identity Core
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDefaultIdentity<IdentityUser>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

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
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["jwtKey"])),
                        ClockSkew = TimeSpan.Zero
                    };

                    //jwtBearerOptions.Events = new JwtBearerEvents()
                    //{
                    //    OnAuthenticationFailed = context =>
                    //    {
                    //        context.NoResult();

                    //        context.Response.StatusCode = 401;
                    //        context.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = context.Exception.Message;
                    //        Console.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);

                    //        return Task.CompletedTask;
                    //    },
                    //    OnTokenValidated = context =>
                    //    {
                    //        Console.WriteLine("OnTokenValidated: " +
                    //            context.SecurityToken);
                    //        return Task.CompletedTask;
                    //    }

                    //};
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
            //services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("127.0.0.1:6379"));

            //特定導向https 5001
            //services.AddHttpsRedirection(options =>
            //{
            //    options.HttpsPort = 5001;
            //});

            //services.AddSingleton<JwtAuth>();

            //HttpRequest
            services.AddHttpClient();

            services.AddTransient<ServiceA>();
            services.AddSingleton<ServiceB>();
            services.AddSingleton<IServiceResolver, ServiceResolver>();

            services.AddHealthChecks();

            //services.AddSingleton<RabbitMQService>();
            //services.AddScoped<RpcClient>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //csv encoding使用big5 dotnet需特別載入(此為Singleton)
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
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
            //Directory.CreateDirectory(Path.GetDirectoryName(Configuration["ImgFileRoot"]));
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

            app.UseHealthChecks("/");

            app.UseMiddleware<RequestResponseLogging>();

            app.UseMvc();
        }
    }
}
