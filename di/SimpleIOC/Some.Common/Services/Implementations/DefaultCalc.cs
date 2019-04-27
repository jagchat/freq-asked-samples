using NLog;
using Some.Common.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Some.Common.Services.Implementations
{
    //NOTE: this is OPTIONAL IMPLEMENTATION and serves as fallback mechanism, 
    //      if no custom implementation of ICalc is configured
    public class DefaultCalc : ICalc
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public string DoCalc(int a, int b)
        {
            logger.Trace("DefaultCalc.DoCalc: started..");
            var r = $"Result from DefaultCalc.DoCalc = {a * b * a * b}"; //some stupid calc
            logger.Trace("DefaultCalc.DoCalc: completed..");
            return r;
        }
    }
}
