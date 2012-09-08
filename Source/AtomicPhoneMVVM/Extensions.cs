

namespace AtomicPhoneMVVM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    
    /// <summary>
    /// Extension methods used in AtomicPhoneMVVM
    /// </summary>
    public static class ExtensionsForNET40
    {
        /// <summary>
        /// Get all custom attributes of a specific type for the current method.
        /// </summary>
        /// <typeparam name="T">The type of custom attributes to find.</typeparam>
        /// <param name="inherit">Do we search inherited methods too.</param>
        /// <returns>A collection of the custom methods.</returns>
        public static IEnumerable<T> GetCustomAttributes<T>(this MethodInfo method, bool inherit)
                where T : Attribute
        {
            return from a in method.GetCustomAttributes(inherit)
                   where a.GetType() == typeof(T)
                   select a as T;
        }

        /// <summary>
        /// Sets the value of a property.
        /// </summary>
        /// <param name="obj">The object to set the property on.</param>
        /// <param name="value">The value of the property.</param>
        public static void SetValue(this PropertyInfo property, object obj, object value)
        {
            property.SetValue(obj, value, null);
        }

        /// <summary>
        /// Gets the value of a property.
        /// </summary>
        /// <param name="obj">The object to get the value from.</param>
        /// <returns>The value.</returns>
        public static object GetValue(this PropertyInfo property, object obj)
        {
            return property.GetValue(obj, null);
        }
    }  

}
