using ASPNETCoreConfigDemo.Models;
using DataLib.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SampleLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCoreConfigDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> log;
        private readonly IConfiguration Configuration;
        private readonly IServiceProvider Provider;

        public HomeController(IServiceProvider provider, ILogger<HomeController> logger, IConfiguration configuration )
        {
            log = logger;
            Provider = provider; //only used for using DI to get instance (of some external class) somewhere in the current class
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            var myKeyValue = Configuration["MyKey"]; //from appsettings.json
            var myUrl = Configuration["MyOptions:MyStartupUrl"]; //from startup.cs
            var myCustomOption1 = Configuration["MyCustomOption1"]; //from custom config provider
            var myCustomJsonOption1 = Configuration["My2ndLevelOptionSet:SetOption1"]; //from custom json config provider
            log.LogInformation($"HomeController.Index => {myCustomJsonOption1}");

            ////Scenario #1
            //OPTION 1: using IServiceProvider
            //use DI for external class (other than controller) and respective interfaces are automatically resolved
            //'DataFactory' has to be registered in Startup
            //var o = (DataFactory) Provider.GetService(typeof(DataFactory));
            //o.UpdateData();

            ////Scenario #1
            //OPTION 2: using HttpContext (not using IServiceProvider)
            DataFactory o2 = (DataFactory)HttpContext.RequestServices.GetService(typeof(DataFactory));
            o2.UpdateData();

            ////Scenario #2
            //using DI in external library loaded dynamically at run-time (using reflection)
            //unknown data type to host
            var o3 = Provider.GetService(Startup.DynType);
            var myMethod = Startup.DynType.GetMethod("UpdateData");
            myMethod.Invoke(o3, null);

            ////Scenario #3
            //register external (dynamically loaded) class to use DI in themselves to access config
            //known interface to host
            var o4 = (IData) Provider.GetService(typeof(IData));
            o4.DoProcess();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
