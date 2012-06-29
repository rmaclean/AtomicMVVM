namespace AtomicStorage
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
#if NETFX_CORE
    using Windows.Storage;
    using Windows.Foundation.Collections;
#endif
    using System.Diagnostics;
#if WINDOWS_PHONE
    using System.IO.IsolatedStorage;
#endif


    public static class DataStoreExtensions
    {
        public static void SaveSettings(this IDataStore store, bool roam = false)
        {
#if NETFX_CORE
            IPropertySet settings;
            if (roam)
            {
                settings = ApplicationData.Current.RoamingSettings.Values;
            }
            else
            {
                settings = ApplicationData.Current.LocalSettings.Values;
            }
#endif
#if WINDOWS_PHONE
            var settings = IsolatedStorageSettings.ApplicationSettings;
#endif

            foreach (var item in store.GetValues())
            {
#if NETFX_CORE
                if (settings.Keys.Contains(item.Key))
#endif
#if WINDOWS_PHONE
                if (settings.Contains(item.Key))
#endif
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
#if NETFX_CORE
            IPropertySet settings;
            if (roam)
            {
                settings = ApplicationData.Current.RoamingSettings.Values;
            }
            else
            {
                settings = ApplicationData.Current.LocalSettings.Values;
            }
#endif
#if WINDOWS_PHONE
            var settings = IsolatedStorageSettings.ApplicationSettings;
#endif

            store.LoadValues(settings);
        }

        private static IDictionary<string, object> GetValues(this IDataStore store)
        {
            var result = new Dictionary<string, object>();

#if NETFX_CORE
            var validMembers = from _ in store.GetType().GetRuntimeProperties()
#endif
#if WINDOWS_PHONE
            var validMembers = from _ in store.GetType().GetProperties()
#endif
                               where _.GetCustomAttributes(typeof(StorageAttribute), false).Any()
                               select _;

            foreach (var member in validMembers)
            {
                var attributeName = (member.GetCustomAttributes(typeof(StorageAttribute), false).Single() as StorageAttribute).Name;
                if (string.IsNullOrWhiteSpace(attributeName))
                {
                    Debug.WriteLine(format: "{0} has no name defined in the storage attribute", args: member.Name);
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
#if NETFX_CORE
            var validMembers = from _ in store.GetType().GetRuntimeProperties()
#endif
#if WINDOWS_PHONE
            var validMembers = from _ in store.GetType().GetProperties()
#endif
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
