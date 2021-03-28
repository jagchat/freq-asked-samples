using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPMVC.Nlog.Test.Sample.Controllers
{
    public class HomeController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        // GET: Home
        public ActionResult Index()
        {
            logger.Trace("this is to trace log file!");
            logger.Error("this is to error log file!");
            return View();
        }
    }
}