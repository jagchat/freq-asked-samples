### Simple Microservices architecture using Azure Service Bus

- Develop two microservices (ProductServce.API and ShoppingCartService.API) to integrate with Azure Service Bus
- ProductService.API publishes event and ShoppingCartService.API handles the event
- Both Services do not know each other
- Used IoC / DI.  Can replace Azure Service Bus with any other Event Bus system
- A Test web app is consumes both of the services


