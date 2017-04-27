using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Some.Common
{
    public class BusinessObject
    {
        public CustomOverrideResult ExecuteOverrides<T>(string methodName)
        {
            var oResult = new CustomOverrideResult();
            var o = UnityConfig.GetInstance<T>();
            if (o != null && ReflectionUtils.IsMethodExists(o, methodName))
            {
                oResult.Data  = ReflectionUtils.InvokeMethod(o, methodName, new object[] { this });
                oResult.IsExecuted = true;
            }

            return oResult;
        }
    }
}
