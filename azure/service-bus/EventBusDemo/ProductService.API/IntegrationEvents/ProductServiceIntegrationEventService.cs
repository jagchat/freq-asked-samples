using EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using ProductService.API.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.API.IntegrationEvents
{
    public class ProductServiceIntegrationEventService : IProductServiceIntegrationEventService
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger<ProductServiceIntegrationEventService> _logger;

        public ProductServiceIntegrationEventService(IEventBus eventBus, ILogger<ProductServiceIntegrationEventService> logger)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void PublishProductSelectedEvent(ProductSelectedIntegrationEvent evnt)
        {
            _logger.LogInformation($"----- Publishing integration event: ProductSelectedEvent {evnt.ProductID}");

            try
            {
                _eventBus.Publish(evnt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Publishing integration event: ProductSelectedEvent ");
            }
        }
    }
}
