using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartService.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CartController: ControllerBase
    {
        private readonly ILogger<CartController> _logger;
        IMemoryCache _memoryCache;

        public CartController(ILogger<CartController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public List<string> Get()
        {
            _logger.LogInformation("CartController.Get: Started...");
            //not a great idea...but for POC
            List<string> lst;
            if (!_memoryCache.TryGetValue(Startup.ProductsSelected_key, out lst))
            {
                lst = new List<string>();
            }
            return lst;
        }

    }
}
