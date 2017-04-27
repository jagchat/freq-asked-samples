using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace app.session.start.stop.msgs
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        protected void Application_Start()
        {
            _logger.Trace("in Application.Start..");
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        public void Application_End()
        {
            _logger.Trace("in Application.End..");
        }

        protected void Session_Start(Object sender, EventArgs e)
        {
            _logger.Trace("in Session.Start..");
        }

        protected void Session_End(Object sender, EventArgs e)
        {
            _logger.Trace("in Session.End..");
        }
    }
}
