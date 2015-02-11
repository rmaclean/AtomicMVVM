//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

namespace AtomicMVVM
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides support functions for Metro Apps
    /// </summary>
    public static class ExtensionsForWinRT
    {
        /// <summary>
        /// Gets the constructor.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The constructor info</returns>
        public static ConstructorInfo GetConstructor(this Type type, Type[] parameters)
        {
            return type.GetTypeInfo().GetConstructor(parameters);
        }

        /// <summary>
        /// Gets the constructor.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The constructor info</returns>
        public static ConstructorInfo GetConstructor(this TypeInfo type, Type[] parameters)
        {
            foreach (var constructor in type.DeclaredConstructors)
            {
                var found = true;
                var constructorParameters = constructor.GetParameters().Select(_ => _.ParameterType).ToArray();

                if (constructorParameters.Length != parameters.Length)
                {
                    continue;
                }

                for (int counter = 0; counter < constructorParameters.Length; counter++)
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