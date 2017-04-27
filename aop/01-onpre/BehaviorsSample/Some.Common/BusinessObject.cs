using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Some.Common
{
    //TODO: fix dirty code.
    public class BusinessObject
    {
        
        public virtual List<Behavior> GetBehaviors()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Behaviors));
            FileStream fs = new FileStream("Behaviors.xml", FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);

            var oBehaviors = (Behaviors)serializer.Deserialize(reader);
            fs.Close();

            return oBehaviors.Items.Where(o => o.ClassType == this.GetType().Name).ToList();
        }

        public virtual List<Method> GetMethods(string methodName)
        {
            List<Method> result = new List<Method>();
            var lstBehaviors = GetBehaviors();
            foreach (var item in lstBehaviors)
            {
                result.AddRange(item.Methods.Where(m => m.Name == methodName));
            }
            return result;
        }

        public virtual void OnPre(string methodName, BusinessObject oBusinessObject)
        {
            var lstMethods = GetMethods(methodName).Where(m => !string.IsNullOrEmpty(m.Pre)).ToList();
            foreach (var item in lstMethods)
            {
                var asm = ReflectionUtils.GetAssembly(item.Assembly);
                if (asm == null)
                {
                    throw new Exception($"Assembly '{item.Assembly}' not found");
                }
                var type = ReflectionUtils.GetType(asm, item.Class);
                if (type == null)
                {
                    throw new Exception($"type '{item.Class}' not found");
                }
                var oInstance = type.GetInstance();
                if (oInstance == null)
                {
                    throw new Exception($"cannot create instance of type '{item.Class}'");
                }
                var mInfo = type.GetMethods().FirstOrDefault(m => m.Name.Equals(item.Pre));
                if (mInfo == null)
                {
                    throw new Exception($"cannot find method '{item.Pre}'");
                }
                mInfo.Invoke(oInstance, new object[] { oBusinessObject });
            }
        }
    }
}
