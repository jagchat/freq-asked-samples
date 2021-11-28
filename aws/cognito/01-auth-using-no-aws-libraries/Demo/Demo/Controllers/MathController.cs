using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Controllers
{
    public class MathController : Controller
    {
        [Authorize] //authentication is needed (whether or not a role exists)
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "dev")] //should have dev role
        public JsonResult Sum(string a, string b)
        {
            var c = int.Parse(a) + int.Parse(b);
            return Json(c);
        }

        [HttpPost]
        [Authorize(Roles = "seniordev")] //should have seniordev role
        public JsonResult Multiply(string a, string b)
        {
            var c = int.Parse(a) * int.Parse(b);
            return Json(c);
        }
    }
}
