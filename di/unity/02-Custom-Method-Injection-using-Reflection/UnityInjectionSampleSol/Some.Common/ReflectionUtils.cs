using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Some.Common
{
    //TODO: fix dirty code
    public static class ReflectionUtils
    {

        public static string GetBinFolderPath()
        {
            return System.IO.Path.GetDirectoryName(
                new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);
        }

        public static Assembly GetAssembly(string AssemblyName)
        {
            try
            {
                var asm = AppDomain.CurrentDomain.GetAssemblies()
                    .SingleOrDefault(p => p.GetName().FullName.Contains(AssemblyName)); //TODO: improve this
                if (asm != null)
                    return asm;
                asm = Assembly.LoadFrom(Path.Combine(GetBinFolderPath(), AssemblyName));
                return asm;
            }
            catch (Exception e)
            {
                //TODO: fix this
                return null;
            }
        }

        public static Type GetType(Assembly asm, string typeName)
        {
            return asm.GetTypes().FirstOrDefault(t => t.FullName == typeName);
        }

        public static bool IsMethodExists(Object o, string methodName)
        {
            var mInfo = o.GetType().GetMethods().FirstOrDefault(m => m.Name.Equals(methodName));
            return !(mInfo == null);
        }

        public static object InvokeMethod(Object o, string methodName, object[] paramsObjects)
        {
            var mInfo = o.GetType().GetMethods().FirstOrDefault(m => m.Name.Equals(methodName));
            if (mInfo == null)
            {
                throw new Exception($"cannot find method '{methodName}'");
            }
            return mInfo.Invoke(o, paramsObjects);
        }

        public static object GetInstance(this Type type)
        {
            return GetInstance<TypeToIgnore>(type, null);
        }

        public static object GetInstance<TArg>(this Type type, TArg argument)
        {
            return GetInstance<TArg, TypeToIgnore>(type, argument, null);
        }

        public static object GetInstance<TArg1, TArg2>(this Type type, TArg1 argument1, TArg2 argument2)
        {
            return GetInstance<TArg1, TArg2, TypeToIgnore>(type, argument1, argument2, null);
        }

        public static object GetInstance<TArg1, TArg2, TArg3>(
            this Type type,
            TArg1 argument1,
            TArg2 argument2,
            TArg3 argument3)
        {
            return InstanceCreationFactory<TArg1, TArg2, TArg3>
                .CreateInstanceOf(type, argument1, argument2, argument3);
        }

        private class TypeToIgnore
        {
        }

        private static class InstanceCreationFactory<TArg1, TArg2, TArg3>
        {
            // This dictionary will hold a cache of object-creation functions, keyed by the Type to create:
            private static readonly Dictionary<Type, Func<TArg1, TArg2, TArg3, object>> _instanceCreationMethods =
                new Dictionary<Type, Func<TArg1, TArg2, TArg3, object>>();

            public static object CreateInstanceOf(Type type, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            {
                CacheInstanceCreationMethodIfRequired(type);

                return _instanceCreationMethods[type].Invoke(arg1, arg2, arg3);
            }

            private static void CacheInstanceCreationMethodIfRequired(Type type)
            {
                // Bail out if we've already cached the instance creation method:
                if (_instanceCreationMethods.ContainsKey(type))
                {
                    return;
                }

                var argumentTypes = new[] {typeof(TArg1), typeof(TArg2), typeof(TArg3)};

                // Get a collection of the constructor argument Types we've been given; ignore any 
                // arguments which are of the 'ignore this' Type:
                Type[] constructorArgumentTypes = argumentTypes.Where(t => t != typeof(TypeToIgnore)).ToArray();

                // Get the Constructor which matches the given argument Types:
                var constructor = type.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    CallingConventions.HasThis,
                    constructorArgumentTypes,
                    new ParameterModifier[0]);

                // Get a set of Expressions representing the parameters which will be passed to the Func:
                var lamdaParameterExpressions = new[]
                {
                    Expression.Parameter(typeof(TArg1), "param1"),
                    Expression.Parameter(typeof(TArg2), "param2"),
                    Expression.Parameter(typeof(TArg3), "param3")
                };

                // Get a set of Expressions representing the parameters which will be passed to the constructor:
                var constructorParameterExpressions = lamdaParameterExpressions
                    .Take(constructorArgumentTypes.Length)
                    .ToArray();

                // Get an Expression representing the constructor call, passing in the constructor parameters:
                var constructorCallExpression = Expression.New(constructor, constructorParameterExpressions);

                // Compile the Expression into a Func which takes three arguments and returns the constructed object:
                var constructorCallingLambda = Expression
                    .Lambda<Func<TArg1, TArg2, TArg3, object>>(constructorCallExpression, lamdaParameterExpressions)
                    .Compile();

                _instanceCreationMethods[type] = constructorCallingLambda;
            }
        }
    }
}
