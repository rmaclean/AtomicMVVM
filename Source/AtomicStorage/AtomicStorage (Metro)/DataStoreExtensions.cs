namespace AtomicStorage
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.RuntimeExtensions;
    using System.Diagnostics;
    using Windows.Storage;
    using Windows.Foundation.Collections;

    public static class DataStoreExtensions
    {
        public static void SaveSettings(this IDataStore store, bool roam = false)
        {
            IPropertySet settings;
            if (roam)
            {
                settings = ApplicationData.Current.RoamingSettings.Values;
            }
            else
            {
                settings = ApplicationData.Current.LocalSettings.Values;
            }

            foreach (var item in store.GetValues())
            {                
                if (settings.Keys.Contains(item.Key))
                {
                    settings[item.Key] = item.Value;
                }
                else
                {
                    settings.Add(item.Key, item.Value);
                }
            }
        }

        public static void LoadSettings(this IDataStore store, bool roam = false)
        {
            IPropertySet settings;
            if (roam)
            {
                settings = ApplicationData.Current.RoamingSettings.Values;
            }
            else
            {
                settings = ApplicationData.Current.LocalSettings.Values;
            }

            store.LoadValues(settings);
        }

        private static IDictionary<string, object> GetValues(this IDataStore store)
        {
            var result = new Dictionary<string, object>();

            var validMembers = from _ in store.GetType().GetRuntimeProperties()
                               where _.GetCustomAttributes(typeof(StorageAttribute), false).Any()
                               select _;

            foreach (var member in validMembers)
            {
                var attributeName = (member.GetCustomAttributes(typeof(StorageAttribute), false).Single() as StorageAttribute).Name;
                if (string.IsNullOrWhiteSpace(attributeName))
                {
                    Debug.WriteLine(format:"{0} has no name defined in the storage attribute", args: member.Name);
                    continue;
                }

                object value = null;
                if (member is PropertyInfo)
                {
                    value = (member as PropertyInfo).GetValue(store, null);
                }                

                if (value != null)
                {
                    result.Add(attributeName, value);
                }
            }

            return result;
        }

        private static void LoadValues(this IDataStore store, IDictionary<string, object> values)
        {
            var validMembers = from _ in store.GetType().GetRuntimeProperties()
                               where _.GetCustomAttributes(typeof(StorageAttribute), false).Any()
                               select _;

            foreach (var member in validMembers)
            {
                var name = (member.GetCustomAttributes(typeof(StorageAttribute), false).Single() as StorageAttribute).Name;

                if (values.Any(_ => _.Key == name))
                {
                    var value = values.Single(_ => _.Key == name).Value;

                    if (member is PropertyInfo)
                    {
                        (member as PropertyInfo).SetValue(store, value, null);
                    }                   
                }
            }
        }
    }
}
