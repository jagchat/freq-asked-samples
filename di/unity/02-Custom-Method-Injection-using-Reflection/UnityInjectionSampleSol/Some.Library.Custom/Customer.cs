using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Some.Library.Interfaces;

namespace Some.Library.Custom
{
    public class Customer : Some.Library.Interfaces.ICustomer
    {
        public string ComputeLabelName(object o)
        {
            var oCustomer = (Some.Library.Gen.Customer) o;
            return oCustomer.FirstName + " - Custom - " + oCustomer.LastName;
        }

        ////if we comment this, it falls back to base
        ////if we uncomment this, this one is priority
        //public void ShowDummyStatus(object o)
        //{
        //    Console.WriteLine("Some.Library.Custom.ShowDummyStatus");
        //}
        
    }
}
