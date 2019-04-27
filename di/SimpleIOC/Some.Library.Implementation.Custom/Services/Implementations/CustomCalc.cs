using NLog;
using Some.Common.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Some.Library.Implementation.Custom.Services.Implementations
{
    public class CustomCalc : ICalc
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public string DoCalc(int a, int b)
        {
            logger.Trace("CustomCalc.DoCalc: started..");
            var r = $"Result from CustomCalc.DoCalc = {a * b}"; //some stupid calc
            logger.Trace("CustomCalc.DoCalc: completed..");
            return r;
        }
    }
}
