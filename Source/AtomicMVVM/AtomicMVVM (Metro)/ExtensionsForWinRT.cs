

namespace AtomicMVVM
{
    using System;
    using System.Reflection;
    using System.Linq;
    using System.Collections.Generic;

    public static class ExtensionsForWinRT
    {
        public static IEnumerable<MethodInfo> GetMethods(this Type type)
        {
            return type.GetTypeInfo().DeclaredMethods;
        }

        public static PropertyInfo GetProperty(this Type type, string propertyName)
        {
            //var typeInfo = type.GetTypeInfo();
            //foreach (var property in typeInfo.DeclaredProperties)
            //{
            //    if (property.Name == propertyName)
            //    {
            //        return property;
            //    }
            //}

            //return null;
            return type.GetTypeInfo().DeclaredProperties.FirstOrDefault(_ => _.Name == propertyName);
        }

        public static MethodInfo GetMethod(this Type type, string methodName, Type[] parameters)
        {
            var results = from m in type.GetTypeInfo().DeclaredMethods
                          where m.Name == methodName
                          let methodParameters = m.GetParameters().Select(_ => _.ParameterType).ToArray()
                          where methodParameters.Length == parameters.Length &&
                                !methodParameters.Except(parameters).Any() &&
                                !parameters.Except(methodParameters).Any()
                          select m;

            return results.FirstOrDefault();
        }

        public static MethodInfo GetMethod(this Type type, string methodName)
        {
            return type.GetMethod(methodName, new Type[] { });
        }

        public static ConstructorInfo GetConstructor(this Type type, Type[] parameters)
        {
            return type.GetTypeInfo().GetConstructor(parameters);
        }

        public static ConstructorInfo GetConstructor(this TypeInfo type, Type[] parameters)
        {
            foreach (var constructor in type.DeclaredConstructors)
            {
                var found = true;
                var constructorParameters = constructor.GetParameters().Select(_ => _.ParameterType).ToArray();

                for (int counter = 0; counter < parameters.Length; counter++)
                {
                    if (constructorParameters[counter] != parameters[counter])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    return constructor;
                }
            }

            return null;
        }
    }
}
