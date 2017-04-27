using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Some.Common;

namespace Some.Library.Base
{
    public class Customer: Some.Library.Gen.Customer
    {
        public string ComputeLabelName()
        {
            OnPre("ComputeLabelName", this);
            var r = FirstName + " - " + LastName;
            return r;
        }

    }
}
