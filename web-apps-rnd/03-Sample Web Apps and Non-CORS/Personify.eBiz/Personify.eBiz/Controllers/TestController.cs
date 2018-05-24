using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Personify.eBiz.Controllers
{
    public class SumInputModel
    {
        public string a { get; set; }
        public string b { get; set; }
    }

    [Produces("application/json")]
    [Route("test")]
    public class TestController : Controller
    {
        [HttpGet]
        [Route("default")]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult GetDefault()
        {
            return Ok("this is test");
        }

        [HttpGet()]
        [Route("square/{id}")]
        [ProducesResponseType(typeof(int), 200)]
        public IActionResult GetSquare(string id)
        {
            var result = Convert.ToInt16(id) * Convert.ToInt16(id);
            return Ok(result);
        }

        [HttpPost()]
        [Route("sum")]
        [ProducesResponseType(typeof(int), 200)]
        public IActionResult GetSum([FromBody] SumInputModel m)
        {
            var result = Convert.ToInt16(m.a) + Convert.ToInt16(m.b);
            return Ok(result);
        }

    }
}