using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http.Controllers;

namespace DynamicEntitiesREST.Infrastructure
{
    public class CustomActionSelector: ApiControllerActionSelector
    {
        public override HttpActionDescriptor SelectAction(HttpControllerContext context)
        {
            var request = new HttpMessageContent(context.Request).HttpRequestMessage;
            MethodInfo method = null;

            //TODO: for GET
            if (request.Method == HttpMethod.Get)
            {
                //TODO: figure out a better way
                if (context.RouteData.Values.Count == 1) //just the entity/controller
                {
                    method = GetActionMethods(context).FirstOrDefault(m => m.Name == "GetAll");
                }
                else if (context.RouteData.Values.Count == 2) //just the entity/controller with id parameter
                {
                    method = GetActionMethods(context).FirstOrDefault(m => m.Name == "GetById");
                }
            }

            //TODO: for POST/PUT/DELETE/Actions: TBD
            //if (request.Method == HttpMethod.Post &&
            //    request.Content.Headers.ContentType.MediaType.Equals("application/json"))
            //{
            //    var json = request.Content.ReadAsStringAsync().Result;

            //    if (!string.IsNullOrWhiteSpace(json))
            //    {
            //    }
            //}

            //TODO: more Route/URL parsing: TBD


            if (method != null)
            {
                return new ReflectedHttpActionDescriptor(context.ControllerDescriptor, method);
            }

            return base.SelectAction(context);
        }

        private static IEnumerable<MethodInfo> GetActionMethods(HttpControllerContext context)
        {
            return context.ControllerDescriptor.ControllerType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public);
            //.Where(m => m.GetCustomAttributes(typeof(HttpPostAttribute)).Any())
            //.Where(m => m.GetParameters().Any());
        }
    }
}