using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LuceneSearchDemo.Service.Library.Employees;
using SearchDemo.Web.Models;

namespace LuceneSearchDemo.Web.Controllers
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
            vm.Results = (new EmployeeSearch()).PerformSearch(SearchText);
            return PartialView("SearchResultsView", vm);
        }

    }
}