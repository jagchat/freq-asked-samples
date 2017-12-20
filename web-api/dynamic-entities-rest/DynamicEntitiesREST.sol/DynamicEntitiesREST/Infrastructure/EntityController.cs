using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace DynamicEntitiesREST.Infrastructure
{

    //TODO: default, but new controllers can be added/discovered via Config/CustomControllerSelector
    public class EntityController : ApiController 
    { 
        
        public HttpResponseMessage GetAll()
        {

            string entityName = this.ControllerContext.RouteData.Values["controller"].ToString();

            var rep = new EntityRepository(entityName);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, rep.GetAll());
            return response;
        }

        public HttpResponseMessage GetById()
        {

            string entityName = this.ControllerContext.RouteData.Values["controller"].ToString();
            string id = this.ControllerContext.RouteData.Values["id"].ToString();

            var rep = new EntityRepository(entityName);
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, rep.GetById(id));
            return response;
        }

    }
}