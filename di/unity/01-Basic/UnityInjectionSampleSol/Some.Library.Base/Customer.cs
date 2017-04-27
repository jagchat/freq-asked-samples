using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Some.Library.Interfaces;

namespace Some.Library.Base
{
    public class Customer : Some.Library.Interfaces.ICustomer
    {
        public string ComputeLabelName()
        {
            return "Some.Library.Base.ComputeLabel";
        }

        public void ShowDummyStatus()
        {
            Console.WriteLine("Some.Library.Base.ShowDummyStatus");
        }
    }
}
