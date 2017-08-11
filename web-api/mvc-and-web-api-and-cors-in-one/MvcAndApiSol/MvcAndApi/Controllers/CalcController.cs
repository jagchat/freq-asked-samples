using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Cors;
using System.Web.Mvc;
using Api;
using Api.Models;

namespace MvcAndApi.Controllers
{
    public class CalcController : Controller
    {
        // GET: Calc
        public ActionResult Index()
        {
            var model = new CalcOutputViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(CalcInputViewModel inputModel)
        {
            var model = (new CalcService()).GetSum(inputModel);
            return View(model);
        }
    }
}