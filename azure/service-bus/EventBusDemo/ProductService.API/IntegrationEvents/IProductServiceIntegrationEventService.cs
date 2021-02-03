using EventBus.Events;
using ProductService.API.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.API.IntegrationEvents
{
    public interface IProductServiceIntegrationEventService
    {
        void PublishProductSelectedEvent(ProductSelectedIntegrationEvent evnt);
    }
}
