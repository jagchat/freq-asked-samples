using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Some.Library.Interfaces;

namespace Some.Library.Base
{
    public class Customer: Some.Library.Gen.Customer
    {
        public string ComputeLabelName()
        {
            var overrideResult = ExecuteOverrides<ICustomer>("ComputeLabelName");
            if (overrideResult.IsExecuted)
            {
                return (string) overrideResult.Data;
            }
            else
            {
                return FirstName + " - " + LastName;
            }
        }

        public void ShowDummyStatus()
        {
            var overrideResult = ExecuteOverrides<ICustomer>("ShowDummyStatus");
            if (!overrideResult.IsExecuted)
            {
                Console.WriteLine("Some.Library.Base.ShowDummyStatus");
            }
            
        }
    }
}
