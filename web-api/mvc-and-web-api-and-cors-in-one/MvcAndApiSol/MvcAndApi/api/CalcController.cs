using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Api;
using Api.Models;

namespace MvcAndApi.api
{
    [RoutePrefix("calc")]
    public class CalcController : ApiController
    {
        [ActionName("sum")]
        public IHttpActionResult GetSum()
        {
            return Json("hello");
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [ActionName("sum")]
        [HttpPost]
        public IHttpActionResult GetSum(CalcInputViewModel inputModel)
        {
            var model = (new CalcService()).GetSum(inputModel);
            return Json(model);
        }
    }
}
