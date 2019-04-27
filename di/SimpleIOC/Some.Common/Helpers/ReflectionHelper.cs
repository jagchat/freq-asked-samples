using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Some.Common.Helpers
{
    public class ReflectionHelper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static Assembly GetAssembly(string AssemblyName, string AssemblyPath)
        {
            logger.Trace($"GetAssembly(): searching assembly {AssemblyName} at {AssemblyPath}");
            try
            {
                var asm = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(p => p.GetName().FullName.Contains(AssemblyName)); //TODO: improve this
                if (asm == null)
                {
                    asm = Assembly.LoadFrom(Path.Combine(AssemblyPath, AssemblyName));
                }
                logger.Trace($"GetAssembly(): found..returning");
                return asm;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"ERROR: at GetAssembly(): Unable to locate/get assembly {AssemblyName} at {AssemblyPath}");
                throw;
            }
        }

        public static Type GetTypeFromAssembly(string AssemblyName, string AssemblyPath, string TypeName)
        {
            logger.Trace($"GetTypeFromAssembly(): searching assembly {AssemblyName} at {AssemblyPath}");
            try
            {
                Type result = null;

                var asm = GetAssembly(AssemblyName, AssemblyPath);
                if (asm == null)
                {
                    logger.Debug(
                        $"WARNING: at GetTypeFromAssembly() - unable to find/load assembly {AssemblyName} at {AssemblyPath}");
                }
                else
                {
                    logger.Trace($"GetTypeFromAssembly(): found assembly...");
                    logger.Trace($"GetTypeFromAssembly(): fetching type...");
                    result = asm.GetTypes().FirstOrDefault(t => t.FullName == TypeName);
                    if (result == null)
                    {
                        logger.Debug(
                            $"WARNING: at GetTypeFromAssembly() - unable to find class/type {TypeName} in {AssemblyName} at {AssemblyPath}");
                    }
                    else
                    {
                        logger.Trace($"GetTypeFromAssembly(): found '{TypeName}'..");
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"ERROR: at GetTypeFromAssembly(): Unable to locate/get assembly {AssemblyName} at {AssemblyPath}");
                throw;
            }
        }
    }
}
