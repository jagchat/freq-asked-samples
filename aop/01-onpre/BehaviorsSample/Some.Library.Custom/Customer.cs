using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Some.Common;

namespace Some.Library.Custom
{
    public class Customer
    {
        public void OnPreComputeLabelName(BusinessObject o)
        {
            var oGen = (Some.Library.Gen.Customer) o;
            oGen.FirstName += " - Pre";
        }
        
    }
}
