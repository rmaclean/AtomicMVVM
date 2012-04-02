

namespace AtomicPhoneMVVM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    
    public static class ExtensionsForNET40
    {
        public static IEnumerable<T> GetCustomAttributes<T>(this MethodInfo method, bool inherit)
                where T : Attribute
        {
            return from a in method.GetCustomAttributes(inherit)
                   where a.GetType() == typeof(T)
                   select a as T;
        }

        public static void SetValue(this PropertyInfo property, object obj, object value)
        {
            property.SetValue(obj, value, null);
        }

        public static object GetValue(this PropertyInfo property, object obj)
        {
            return property.GetValue(obj, null);
        }
    }  

}
