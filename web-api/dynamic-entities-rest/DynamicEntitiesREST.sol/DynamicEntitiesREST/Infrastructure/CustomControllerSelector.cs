using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace DynamicEntitiesREST.Infrastructure
{
    public class CustomControllerSelector: DefaultHttpControllerSelector
    {

        private readonly HttpConfiguration _configuration;

        public CustomControllerSelector(HttpConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            
            var controllerName = base.GetControllerName(request);
            //TODO: extensible/external controllers can be identified here, if they are in external assemblies
            var matchedController = Type.GetType("DynamicEntitiesREST.Infrastructure.EntityController");

            return new HttpControllerDescriptor(_configuration, controllerName, matchedController);
        }
    }
}