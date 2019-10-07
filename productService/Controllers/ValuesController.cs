﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using productService.Models;
using productService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using static productService.Startup;

namespace productService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly IConfiguration config;
        private readonly ProductDbContext productDb;
        private readonly IConnectionMultiplexer redis;
        private readonly IService service;
        private readonly IHttpClientFactory clientFactory;
        private readonly RpcClient rpcClient;

        public ValuesController(ILogger<ValuesController> logger, IConfiguration config,
              IHttpClientFactory clientFactory,
             IServiceResolver iserviceResolver, RpcClient rpcClient)
        { //IConnectionMultiplexer redis, ProductDbContext productDb,
            this.logger = logger;
            this.config = config;
            //this.productDb = productDb;
            //this.redis = redis;
            this.service = iserviceResolver.GetServiceByName("B");
            this.clientFactory = clientFactory;
            this.rpcClient = rpcClient;
        }

        public class T
        {
            public int age { set; get; }
            public string name { set; get; }
            public DateTime time { set; get; }
        }

        public class PostData
        {
            public string id { set; get; }
        }

        // GET api/values
        //[CustomTestAuthorizationFilter]
        //[Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://XXXX");

            request.Content = new StringContent(JsonConvert.SerializeObject(
                new PostData
                {
                    id = "123123"
                }
                ), Encoding.UTF8, "application/json");

            var client = clientFactory.CreateClient();
            //client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.SendAsync(request);
            var data = await response.Content.ReadAsStringAsync();



            //var a =HttpContext.User.Claims.ToList();
            //service.ToString();

            //var ac = Request;
            //Response.Headers.Add("ttt", "addheader");
            //Response.Cookies.Append("t", "sd");

            //var a = config["jwtKey"];

            //logger.LogWarning("Test 1111111111111");

            //var rnd = new Random();

            //productDb.Products.Add(new Product()
            //{
            //    Name = "product" + DateTime.Now.ToString("MMddHHmmss"),
            //    Price = rnd.Next(1, 100),
            //    //CreatedTime = DateTime.Now
            //    Note = "test" + rnd.Next(1, 100)

            //});

            //productDb.SaveChanges();


            //IDatabase db = redis.GetDatabase(14);

            //db.StringSet("test", JsonConvert.SerializeObject(new T()
            //{
            //    name = "asd123我",
            //    age = 34,
            //    time = DateTime.Now
            //}));

            //            var t = JsonConvert.DeserializeObject<T>(db.StringGet("test"));

            // db.StringIncrement("test");

            return new string[] { "value1", "value2" };
        }

        
        [HttpGet("sendMsg")]
        public async Task<string> SendMsg(string msg)
        {

            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                string message = "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "hello",
                                     basicProperties: null,
                                     body: body);
            }


            return "ok";
        }

        [HttpGet("sendRPCMsg")]
        public async Task<string> SendRPCMsg(string msg)
        {
            var response =  await rpcClient.Call(msg);


            return response;
        }

        // POST api/values

        [HttpPost]
        public List<T> Post(x x)
        {

            var xx = new T()
            {
                age = 7,
                name = "testesttesttesttesttesttesttesttesttesttestt" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                time = DateTime.Now
            };


            var ts = new List<T>();

            for (var i = 0; i < 50; i++)
            {
                ts.Add(xx);
            }

            //轉換class to string，方便寫log
            var xxStr = ObjectDumper.Dump(xx, DumpStyle.CSharp);

            return ts;
        }

        public class x
        {
            public string aa { set; get; }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
