using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using productService.Models;

namespace productService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private readonly ProductDbContext productDb;

        public ValuesController(ILogger<ValuesController> logger, IConfiguration config, ProductDbContext productDb)
        {
            _logger = logger;

            _config = config;
            this.productDb = productDb;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            //_logger.LogWarning("Test 1111111111111");

            var rnd = new Random();

            productDb.Products.Add(new Product()
            {
                Name = "product" + DateTime.Now.ToString("MMddHHmmss"),
                Price = rnd.Next(1, 100),
                //CreatedTime = DateTime.Now
                Note = "test" +rnd.Next(1,100)
                
            });

            productDb.SaveChanges();

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value"+id;
        }

        // POST api/values

        [HttpPost]
        public string Post( x x)
        {
            return "post rRequest Method: POST\nStatus Code: 200 OK\nRemote Address: [::1]:5000\nReferrer Policy: no-referrer-when-downgrade\nAccess-Control-Allow-Origin: *\nContent-Encoding: gzip\nContent-Type: text/plain; charset=utf-8\nDate: Fri, 05 Jul 2019 05:33:56 GMT\nServer: Kestrel\nTransfer-Encoding: chunked\nVary: Accept-Encoding\nProvisional headers are shown\nAccept: application/json, text/plain, */*\nContent-Type: application/json\nOrigin: http://localhost:8100\nReferer: http://localhost:8100/\nUser-Agent: Mozilla/5.0 (Macin\", \"Request Method: POST\nStatus Code: 200 OK\nRemote Address: [::1]:5000\nReferrer Policy: no-referrer-when-downgrade\nAccess-Control-Allow-Origin: *\nContent-Encoding: gzip\nContent-Type: text/plain; charset=utf-8\nDate: Fri, 05 Jul 2019 05:33:56 GMT\nServer: Kestrel\nTransfer-Encoding: chunked\nVary: Accept-Encoding\nProvisional headers are shown\nAccept: application/json, text/plain, */*\nContent-Type: application/json\nOrigin: http://localhost:8100\nReferer: http://localhost:8100/\nUser-Agent: Mozilla/5.0 (Macinturn";
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
