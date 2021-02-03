using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductService.API.IntegrationEvents.Events
{
    public class ProductSelectedIntegrationEvent: IntegrationEvent
    {
        public string ProductID { get; set; }
        public ProductSelectedIntegrationEvent(string productId) => ProductID = productId;
    }
}
