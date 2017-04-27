using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NLog;

namespace app.session.start.stop.msgs.Controllers
{
    public class HomeController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        // GET: Home
        public ActionResult Index()
        {
            logger.Trace("In Home.Index");
            return View();
        }

        public EmptyResult EndSession()
        {
            Session.Abandon();
            return new EmptyResult();
        }
    }
}