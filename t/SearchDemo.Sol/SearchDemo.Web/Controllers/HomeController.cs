using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SearchDemo.Web.Models;

namespace SearchDemo.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string SearchText)
        {
            var vm = new SearchViewModel();
            vm.Results.Add(new SearchResultItem() { Text = "Search Hit 1", Description = "Search Description 1", Link = "http://searchlink1" });
            vm.Results.Add(new SearchResultItem() { Text = "Search Hit 2", Description = "Search Description 2", Link = "http://searchlink1" });
            vm.Results.Add(new SearchResultItem() { Text = "Search Hit 3", Description = "Search Description 3", Link = "http://searchlink1" });
            vm.Results.Add(new SearchResultItem() { Text = "Search Hit 4", Description = "Search Description 4", Link = "http://searchlink1" });
            vm.Results.Add(new SearchResultItem() { Text = "Search Hit 5", Description = "Search Description 5", Link = "http://searchlink1" });
            vm.Results.Add(new SearchResultItem() { Text = "Search Hit 6", Description = "Search Description 6", Link = "http://searchlink1" });
            vm.Results.Add(new SearchResultItem() { Text = "Search Hit 7", Description = "Search Description 7", Link = "http://searchlink1" });
            return PartialView("SearchResultsView", vm);
        }
    }
}