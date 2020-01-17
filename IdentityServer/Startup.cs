// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using IdentityServer.Custom;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.ResponseHandling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace IdentityServer
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var ic = ConnectionMultiplexer.Connect(Configuration["Redis"].ToString());

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

            services.AddSingleton<IConnectionMultiplexer>(ic);

            //services.AddDataProtection().SetApplicationName("idsrv").PersistKeysToFileSystem(new DirectoryInfo(Configuration["DataProtectionPath"]));
            services.AddDataProtection().SetApplicationName("idsrv").PersistKeysToStackExchangeRedis(ic, "DataProtection-Keys");

            // uncomment, if you want to add an MVC-based UI
            services.AddControllersWithViews();

            var builder = services.AddIdentityServer((otp) =>
                {
                    otp.UserInteraction = new IdentityServer4.Configuration.UserInteractionOptions()
                    {
                        //LoginUrl = "http://www.google.com"
                        //ConsentUrl = "http://www.google.com",
                    };
                    otp.Endpoints = new IdentityServer4.Configuration.EndpointsOptions()
                    {
                        EnableCheckSessionEndpoint = false,
                        EnableEndSessionEndpoint = false,
                        EnableDeviceAuthorizationEndpoint = false
                    };
                    //otp.IssuerUri =  
                    //otp.PublicOrigin = "http://localhost:8081";
                })
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                })
                .AddOperationalStore(options =>
                {
                    //user by db
                    //options.ConfigureDbContext = b => b.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                    //options.EnableTokenCleanup = true;
                    //options.TokenCleanupInterval = 5;

                    options.RedisConnectionMultiplexer = ic;
                })
                .AddRedisCaching(options =>
                {
                    options.RedisConnectionMultiplexer = ic;
                })
                .AddClientStoreCache<ClientStore>()
                .AddResourceStoreCache<ResourceStore>()
                .AddTestUsers(TestUsers.Users);

            builder.Services.AddScoped<IAuthorizeResponseGenerator, WebcommAuthorizeResponseGenerator>();

            // not recommended for production - you need to store your key material somewhere secure
            //builder.AddDeveloperSigningCredential();

            var certPath = (Environment.IsDevelopment()) ? Path.Combine(Directory.GetCurrentDirectory(), Configuration["Certificate:Path"]) : Configuration["Certificate:Path"];

            builder.AddSigningCredential(new X509Certificate2(certPath, Configuration["Certificate:Password"]));

            //services.AddSession();

            //services.AddStackExchangeRedisCache((otp) =>
            //{
            //    otp.Configuration = Configuration["Redis"].ToString() + ",defaultDatabase=1";
            //});

            //services.AddAuthentication("cookie")
            //.AddCookie("cookie", options =>
            //{
            //    //options.ExpireTimeSpan = new System.TimeSpan(0, 1, 10);
            //    //options.SessionStore = new RedisCacheTicketStore(new Microsoft.Extensions.Caching.StackExchangeRedis.RedisCacheOptions()
            //    //{
            //    //    Configuration = Configuration["Redis"].ToString() + ",defaultDatabase=1"
            //    //});

            //    var idp = new ServiceCollection().AddDataProtection().PersistKeysToStackExchangeRedis(ic, "DataProtection-Keys").Services.BuildServiceProvider().GetRequiredService<IDataProtectionProvider>();
            //    options.DataProtectionProvider = idp;
            //});


        }

        public void Configure(IApplicationBuilder app)
        {
            InitializeDatabase(app);

            app.Use(async (context, next) =>
            {
                System.Console.WriteLine(FormatHeaders(context.Request.Headers));

                await next.Invoke();

            });

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            //app.UseCors();
            //app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private string FormatHeaders(IHeaderDictionary headers)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("[");
            foreach (var (key, value) in headers)
            {
                stringBuilder.Append($"{key}: {value}; ");
            }
            stringBuilder.AppendLine("]");

            return stringBuilder.ToString();
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                if (!context.Clients.Any())
                {
                    foreach (var client in Config.Clients)
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.Ids)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.Apis)
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
