

namespace AtomicMVVM
{
    using System;
    using System.Linq;
    using System.Reflection;

    public static class ExtensionsForWinRT
    {        
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
