using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductService.API.IntegrationEvents;
using ProductService.API.IntegrationEvents.Events;
using ProductService.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductServiceIntegrationEventService _productIntegrationEventService;

        public ProductController(ILogger<ProductController> logger, IProductServiceIntegrationEventService productIntegrationEventService)
        {
            _logger = logger;
            _productIntegrationEventService = productIntegrationEventService;
        }

        [HttpGet]
        public List<Product> Get()
        {
            _logger.LogInformation("ProductController.Get: Started...");
            var lst = new List<Product>();
            lst.Add(new Product() { ProductId = "1", Name = "eBook" });
            lst.Add(new Product() { ProductId = "2", Name = "CD/DVD" });
            lst.Add(new Product() { ProductId = "3", Name = "Hardcovered Book" });
            _logger.LogInformation("ProductController.Get: Completed...");
            return lst;
        }

        [Route("SelectProduct")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult SelectProduct(Product m)
        {
            _logger.LogInformation($"ProductController.SelectProduct: Started (ProductId: {m.ProductId}...");
            _productIntegrationEventService.PublishProductSelectedEvent(new ProductSelectedIntegrationEvent(m.ProductId));
            _logger.LogInformation($"ProductController.SelectProduct: Completed...");
            return Ok(new { msg = "ok" });
        }
    }
}
