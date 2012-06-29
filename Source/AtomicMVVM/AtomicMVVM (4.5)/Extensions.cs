

namespace AtomicMVVM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
#if WINDOWS_PHONE
    using ActionCommand = Tuple<string, System.Action>;
#else
    using ActionCommand = System.Tuple<string,System.Action>;
#endif
    using System.Reflection;

    public static class AtomicMVVMExtensions
    {
        public static void ExecuteGlobalCommand<TShell>(this Bootstrapper<TShell> bootstrapper, string commandId)            
            where TShell : IShell
        {
            bootstrapper.GlobalCommands.Single(_ => _.Item1 == commandId).Item2();
        }

        public static void Add(this List<ActionCommand> globalCommands, string commandId, Action action)
        {
            var actionCommand = new ActionCommand(commandId, action);
            globalCommands.Add(actionCommand);
        }
    }

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
