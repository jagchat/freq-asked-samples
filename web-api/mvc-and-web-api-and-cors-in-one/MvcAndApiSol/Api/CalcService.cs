using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Api.Models;

namespace Api
{
    public class CalcService
    {
        public CalcOutputViewModel GetSum(CalcInputViewModel m)
        {
            return new CalcOutputViewModel()
            {
                x = m.x,
                y = m.y,
                result = m.x + m.y
            };
        }
    }
}
