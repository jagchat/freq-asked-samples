using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using ServerApi.Models;

namespace ServerApi.api
{
    [RoutePrefix("calc")]
    public class CalcController : ApiController
    {
        [ActionName("sum")]
        public IHttpActionResult GetSum()
        {
            return Json("hello");
        }

        //[EnableCors(origins: "*", headers: "*", methods: "*")] //configured in web.config
        [ActionName("sum")]
        [HttpPost]
        public IHttpActionResult GetSum(CalcInputViewModel m)
        {
            var model = new CalcOutputViewModel()
            {
                x = m.x,
                y = m.y,
                result = m.x + m.y
            };
            return Json(model);
        }

    }
}