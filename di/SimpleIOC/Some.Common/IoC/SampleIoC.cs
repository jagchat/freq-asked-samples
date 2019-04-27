using NLog;
using Some.Common.Helpers;
using Some.Common.Services.Implementations;
using Some.Common.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Some.Common.IoC
{
    public class SampleIoC : ISampleIoC, IFromMapper, IToMapper
    {
        private const string DEFAULT_IOC_ASSEMBLY = "Some.Common.dll";
        private const string DEFAULT_IOC_ICALC_CLASS = "Some.Common.Services.Implementations.DefaultCalc";

        private static Logger logger = LogManager.GetCurrentClassLogger();

        Dictionary<Type, Type> registeredTypes = new Dictionary<Type, Type>();
        Dictionary<Type, object> instances = new Dictionary<Type, object>();
        Type tmp;
        bool isSingletonMode;

        private static SampleIoC container = null;
        private static bool isTypesRegistered = false;

        private static object lockObjectGetContainer = new object();
        public static SampleIoC GetContainer()
        {
            if (container == null)
            {
                lock (lockObjectGetContainer)
                {
                    if (container == null)
                    {
                        container = new SampleIoC();
                    }
                }
            }
            return container;
        }

        private static object lockObjectRegisterTypes = new object();
        public static void RegisterTypes()
        {
            if (!isTypesRegistered)
            {
                lock (lockObjectRegisterTypes)
                {
                    if (!isTypesRegistered)
                    {
                        logger.Trace("Registering types: started..");
                        var iocAssembly = Utils.GetAppConfigValue<string>("IoC_Impl_Assembly", DEFAULT_IOC_ASSEMBLY);
                        var iocICalcClass = Utils.GetAppConfigValue<string>("IoC_Impl_ICalc_Class", DEFAULT_IOC_ICALC_CLASS);
                        if (iocAssembly == DEFAULT_IOC_ASSEMBLY && iocICalcClass == DEFAULT_IOC_ICALC_CLASS)
                        {
                            //load from current assembly

                            //GetContainer().UseSingleton().Register<ICalc, DefaultCalc>(); //if singleton
                            GetContainer().Register<ICalc, DefaultCalc>();
                        }
                        else
                        {
                            //load from external assembly
                            GetContainer().RegisterTypeFromAssembly<ICalc>(iocAssembly, Helpers.Utils.GetBinFolderPath(), iocICalcClass);
                        }
                        isTypesRegistered = true;
                        logger.Trace("Registering types: completed");
                    }
                }
            }
        }

        public static ICalc GetCalcInstance()
        {
            RegisterTypes();
            return GetContainer().Resolve<ICalc>();
        }

        public IToMapper For<T>()
        {
            tmp = typeof(T);
            return this;
        }

        public Dictionary<Type, Type> GetAllRegisteredTypes()
        {
            return registeredTypes;
        }

        public IFromMapper Inject<T>()
        {
            if (registeredTypes.ContainsKey(tmp))
                registeredTypes[tmp] = typeof(T);
            else
            {
                registeredTypes.Add(tmp, typeof(T));
                tmp = null;
            }

            return this;
        }

        public IFromMapper Inject(string AssemblyName, string ClassName)
        {
            Type t = Type.GetType($"{ClassName}, {AssemblyName}");
            if (t == null)
            {
                t = ReflectionHelper.GetTypeFromAssembly(AssemblyName, Helpers.Utils.GetBinFolderPath(), ClassName);
                if (t == null)
                {
                    throw new Exception($"Could not find type '{ClassName}, {AssemblyName}'");
                }
            }
            if (registeredTypes.ContainsKey(tmp))
                registeredTypes[tmp] = t;
            else
            {
                registeredTypes.Add(tmp, t);
                tmp = null;
            }

            return this;
        }

        public IFromMapper Inject(Type t)
        {
            if (registeredTypes.ContainsKey(tmp))
                registeredTypes[tmp] = t;
            else
            {
                registeredTypes.Add(tmp, t);
                tmp = null;
            }

            return this;
        }

        public bool IsRegistered<Type>()
        {
            return registeredTypes.Any(a => a.Key == typeof(Type));
        }

        public IFromMapper Register<Tfrom, TTo>() where TTo : Tfrom
        {
            return For<Tfrom>().Inject<TTo>();
        }

        public T Resolve<T>() where T : class
        {
            if (!registeredTypes.Any())
            {
                var err = "Unable to resolve using IoC. No entity has been registered yet";
                logger.Error(err);
                throw new Exception(err);
            }

            T s = (T)ResolveParameter(typeof(T));

            if (isSingletonMode && !instances.ContainsKey(typeof(T)))
                instances.Add(typeof(T), s);

            return s;
        }

        private object ResolveParameter(Type type)
        {
            try
            {
                Type resolved = null;

                if (isSingletonMode && instances.ContainsKey(type))
                {
                    return instances.First(f => f.Key == type).Value;
                }
                else
                    resolved = registeredTypes[type];

                var cnstr = resolved.GetConstructors().First();
                var cnstrParams = cnstr.GetParameters().Where(w => w.GetType().IsClass);

                // If constructor hasn't parameter, Create an instance of object
                if (!cnstrParams.Any())
                    return Activator.CreateInstance(resolved);

                var paramLst = new List<object>(cnstrParams.Count());

                // Iterate through parameters and resolve each parameter
                for (int i = 0; i < cnstrParams.Count(); i++)
                {
                    var paramType = cnstrParams.ElementAt(i).ParameterType;
                    var resolvedParam = ResolveParameter(paramType);
                    paramLst.Add(resolvedParam);
                }

                return cnstr.Invoke(paramLst.ToArray());
            }
            catch (Exception)
            {
                var err = string.Format("'{0}' Cannot be resolved. Check your registered types", type.FullName);
                logger.Error(err);
                throw new Exception(err);
            }

        }

        public IFromMapper UseSingleton()
        {
            isSingletonMode = true;
            return this;
        }

        public IFromMapper RegisterTypeFromAssembly<Tfrom>(string AssemblyName, string AssemblyPath, string ClassName)
        {
            if (!IsRegistered<Tfrom>())
            {
                logger.Trace($"Registering {ClassName} from {AssemblyPath}\\{AssemblyName} ");
                //just load
                Type t = ReflectionHelper.GetTypeFromAssembly(AssemblyName, AssemblyPath, ClassName);
                return For<Tfrom>().Inject(t);
                //return For<Tfrom>().Inject(AssemblyName, ClassName);
            }
            return this;
        }

    }
}
