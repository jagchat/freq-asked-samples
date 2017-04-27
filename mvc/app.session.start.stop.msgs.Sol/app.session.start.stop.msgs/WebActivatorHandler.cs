using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;

namespace app.session.start.stop.msgs
{
    public class WebActivatorHandler
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static void Start()
        {
            logger.Trace("WebActivator.Start: Started..");
        }
        public static void Shutdown()
        {
            logger.Trace("WebActivator.Shutdown: Stopped..");
        }
    }
}