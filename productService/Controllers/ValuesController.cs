using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using productService.Models;
using productService.Services;
using StackExchange.Redis;

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

        public ValuesController(ILogger<ValuesController> logger, IConfiguration config, ProductDbContext productDb, IConnectionMultiplexer redis)
        {
            this.logger = logger;
            this.config = config;
            this.productDb = productDb;
            this.redis = redis;
        }

        public class T
        {
            public int age { set; get; }
            public string name { set; get; }
            public DateTime time { set; get; }
        }

        // GET api/values
        //[CustomTestAuthorizationFilter]
        //[Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var a =HttpContext.User.Claims.ToList();

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

            return new string[] { "value1", "value2"};
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value"+id;
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

            for(var i = 0; i < 50; i++)
            {
                ts.Add(xx);
            }
            

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
