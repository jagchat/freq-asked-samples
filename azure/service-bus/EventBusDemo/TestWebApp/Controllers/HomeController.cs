using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TestWebApp.Helpers.Models;
using TestWebApp.Models;

namespace TestWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Products()
        {
            return View();
        }

        [HttpPost]
        [Route("Home/Products")]
        public JsonResult ProductAddToCart([FromBody] AddProductToCartInputModel m)
        {
            var api = new Helpers.API.Products();
            var result = api.AddProductToCartAsync(m).Result;

            return Json(result);
        }

        public IActionResult Cart()
        {
            var api = new Helpers.API.Cart();
            var result = api.FetchCartData().Result;

            return View(result);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
