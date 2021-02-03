using EventBus.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ShoppingCartService.API.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartService.API.IntegrationEvents.EventHandling
{
    public class ProductSelectedIntegrationEventHandler : IIntegrationEventHandler<ProductSelectedIntegrationEvent>
    {
        ILogger<ProductSelectedIntegrationEventHandler> _logger;
        IMemoryCache _memoryCache;

        public ProductSelectedIntegrationEventHandler(ILogger<ProductSelectedIntegrationEventHandler> logger, IMemoryCache memoryCache)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _memoryCache = memoryCache;
        }

        public Task Handle(ProductSelectedIntegrationEvent @event)
        {
            _logger.LogInformation("ProductSelectedIntegrationEventHandler.Handle: Started...");
            _logger.LogInformation($"----- Handling integration event: ProductSelectedEvent {@event.ProductID}");
            
            //Just for POC
            List<string> lst;
            if (!_memoryCache.TryGetValue(Startup.ProductsSelected_key, out lst))
            {
                lst = new List<string>();
            }
            lst.Add(@event.ProductID);
            _memoryCache.Set(Startup.ProductsSelected_key, lst);


            _logger.LogInformation("ProductSelectedIntegrationEventHandler.Handle: Completed...");
            return Task.CompletedTask;
        }
    }
}
