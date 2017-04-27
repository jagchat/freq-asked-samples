using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
namespace Some.Common
{
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            container.AddNewExtension<TypeTrackingExtension>();
            container.LoadConfiguration();
            RegisterTypes(container);
            return container;
        });

        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        //Get instance of specified interface/class
        public static T GetInstance<T>()
        {
            T o = default(T);
            if (GetConfiguredContainer().CanResolve<T>())
            {
                o = GetConfiguredContainer().Resolve<T>();
            }
            return o;
        }

        //Get instance of specified interface/class based on mapping name
        public static T GetInstance<T>(string name)
        {
            T o = default(T);
            if (GetConfiguredContainer().CanResolve<T>(name))
            {
                o = GetConfiguredContainer().Resolve<T>(name);
            }
            return o;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here
            // container.RegisterType<IProductRepository, ProductRepository>();
        }
    }
}
