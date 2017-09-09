using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using d3sample.Models;

namespace d3sample.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetJson()
        {
            var outputViewModel = new List<OutputItem>();
            var o = new OutputItem();
            o.name = "Top Level";
            o.parent = "null";
            o.children = new List<OutputItem>();

            o.children.Add(new OutputItem() { name = "Level 2: A", parent = "Top Level" });
            o.children[0].children = new List<OutputItem>();
            o.children[0].children.Add(new OutputItem() { name = "Son of A", parent = "Level 2: A" });
            o.children[0].children.Add(new OutputItem() { name = "Daughter of A", parent = "Level 2: A" });

            o.children.Add(new OutputItem() { name = "Level 2: B", parent = "Top Level" });
            outputViewModel.Add(o);

            return Json(outputViewModel, JsonRequestBehavior.AllowGet);

            //// ----- direct json
            //var s = @"[
            //{
            //    ""name"": ""Top Level"",
            //    ""parent"": ""null"",
            //    ""children"": [
            //        {
            //            ""name"": ""Level 2: A"",
            //            ""parent"": ""Top Level"",
            //            ""children"": [
            //                {
            //                    ""name"": ""Son of A"",
            //                    ""parent"": ""Level 2: A""
            //                },
            //                {
            //                    ""name"": ""Daughter of A"",
            //                    ""parent"": ""Level 2: A""
            //                }
            //            ]
            //        },
            //        {
            //            ""name"": ""Level 2: B"",
            //            ""parent"": ""Top Level""
            //        }
            //    ]
            //}
            //]";

            //return Content(s);
        }

    }
}