using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Some.Common.Helpers
{
    public class Utils
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static string GetBinFolderPath() //TODO: refactor
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            System.IO.DirectoryInfo oDir =
                new System.IO.DirectoryInfo(
                    System.IO.Path.GetDirectoryName(asm.GetName().CodeBase.TrimStart("file:///".ToCharArray())));
            var s = oDir.FullName;
            logger.Debug($"Calculated Shell bin folder path: {s}");
            return s;

            //var s = System.Web.HttpRuntime.BinDirectory; //for web?
            //logger.Debug($"Calculated Shell bin folder path: {s}");
            //return s;

        }

        public static T GetAppConfigValue<T>(string appSetting, T defaultValue = default(T))
        {
            string valueFromConfig = ConfigurationManager.AppSettings[appSetting];

            if (!string.IsNullOrEmpty(valueFromConfig))
            {
                return GetValue<T>(valueFromConfig);
            }

            return defaultValue;
        }

        public static T GetValue<T>(object value)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
                return default(T);

            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
