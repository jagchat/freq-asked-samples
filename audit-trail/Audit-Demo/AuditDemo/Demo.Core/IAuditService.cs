using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Core
{
    public interface IAuditService
    {
        public void Publish(string msg, Dictionary<string, object> options = null);
    }
}
