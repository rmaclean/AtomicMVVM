//-----------------------------------------------------------------------
// Project: AtomicPhoneMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------  
namespace AtomicPhoneMVVM
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using System.Windows;
    using System.Reflection;
    using System.Diagnostics;
    using System.Windows.Media;

    public static class Bootstrapper
    {
        public static void BindData<T>(this PhoneApplicationPage page)
        {
            BindData<T, object>(page, null);
        }

        private static IEnumerable<ButtonBase> FindButtons(DependencyObject controlToSearch)
        {
            Debug.WriteLine("searching: {0}", controlToSearch.GetType().ToString());
            var result = new List<ButtonBase>();
            var childCount = VisualTreeHelper.GetChildrenCount(controlToSearch);
            Debug.WriteLine(childCount);

            for (int count = 0; count < childCount; count++)
            {
                var child = VisualTreeHelper.GetChild(controlToSearch, count);
                Debug.WriteLine(child.GetType().ToString());

                var button = child as ButtonBase;
                if (button != null &&
                    string.IsNullOrWhiteSpace(button.Name) &&
                    !string.IsNullOrWhiteSpace((string)button.GetValue(XAMLExtensions.CommandNameProperty)))
                {
                    result.Add(button);
                }

                var list = child as ItemsControl;
                if (list != null)
                {
                    Debug.WriteLine("in list with count: {0}", list.Items.Count);
                    
                    foreach (var item in list.ItemsSource)
                    {                             
                        var listItem = list.ItemContainerGenerator.ContainerFromItem(item);
                        //Debug.WriteLine(listItem.GetType().ToString());
                    }

                    //foreach (var item in list.Items)
                    //{
                    //    var element = item as ListBoxItem;
                    //    result.AddRange(FindButtons(element));
                    //}
                }

                result.AddRange(FindButtons(child));
            }

            return result;
        }

        public static void BindExtendedControls(this PhoneApplicationPage page)
        {
            if (page.DataContext != null)
            {
                var data = page.DataContext as CoreData;
                if (data != null)
                {
                    BindExtendedControls(page, data, GetValidMethods(data));
                }
            }
        }

        private static void BindExtendedControls(PhoneApplicationPage page, CoreData viewModel, IEnumerable<MethodInfo> validMethods)
        {
            var buttons = FindButtons(page.Content).OrderBy(_ => (string)_.GetValue(XAMLExtensions.CommandNameProperty)).ToList();
            var lastname = string.Empty;
            MethodInfo method = null;
            foreach (var button in buttons)
            {
                var buttonName = (string)button.GetValue(XAMLExtensions.CommandNameProperty);
                if (lastname != buttonName || method == null)
                {
                    lastname = buttonName;
                    method = validMethods.SingleOrDefault(_ => _.Name == buttonName);
                }

                BindButton(viewModel, method, button);
            }
        }

        public static void BindData<T, Y>(this PhoneApplicationPage page, Y data)
        {
            var viewModelName = typeof(T).AssemblyQualifiedName;

            CoreData viewModel;
            if (data == null)
            {
                viewModel = typeof(T).GetConstructor(Type.EmptyTypes).Invoke(null) as CoreData;
            }
            else
            {
                viewModel = typeof(T).GetConstructor(new[] { typeof(Y) }).Invoke(new object[] { data }) as CoreData;
            }

            if (typeof(IPushMessage).IsAssignableFrom(page.GetType()))
            {
                viewModel.Page = (IPushMessage)page;
            }

            var validMethods = GetValidMethods(viewModel);

            foreach (var method in validMethods)
            {
                var attributes = from _ in method.GetCustomAttributes<TriggerPropertyAttribute>(false)
                                 orderby _.Order ascending
                                 select _;

                foreach (var attribute in attributes)
                {
                    AddTrigger(viewModel, attribute.PropertyNames, method.Name);
                }

                var control = page.FindName(method.Name);

                BindButton(viewModel, method, control);
            }

            if (page.ApplicationBar != null && page.ApplicationBar.Buttons != null)
            {
                var appBarMethods = (from m in viewModel.GetType().GetMethods()
                                     where !m.IsSpecialName &&
                                           m.ReturnType == typeof(void) &&
                                           m.DeclaringType != typeof(CoreData) &&
                                           m.GetCustomAttributes<AppBarCommandAttribute>(false).Any()
                                     select m).ToList();

                foreach (var method in appBarMethods)
                {
                    IApplicationBarMenuItem selectedAppBarItem = null;
                    var itemText = method.GetCustomAttributes<AppBarCommandAttribute>(false).Single().AppBarText;
                    foreach (ApplicationBarIconButton appBarItem in page.ApplicationBar.Buttons)
                    {
                        if (appBarItem.Text == itemText)
                        {
                            selectedAppBarItem = appBarItem;
                            break;
                        }
                    }

                    foreach (ApplicationBarMenuItem appBarItem in page.ApplicationBar.MenuItems)
                    {
                        if (appBarItem.Text == itemText)
                        {
                            selectedAppBarItem = appBarItem;
                            break;
                        }
                    }

                    if (selectedAppBarItem == null)
                    {
                        continue;
                    }

                    var canExecuteExists = false;
                    var canExecuteMethod = viewModel.GetType().GetMethod("Can" + method.Name, Type.EmptyTypes);
                    if (canExecuteMethod != null)
                    {
                        canExecuteExists = canExecuteMethod.ReturnType == typeof(bool);
                    }

                    if (canExecuteExists)
                    {
                        var reevaluateAttributes = from _ in canExecuteMethod.GetCustomAttributes<ReevaluatePropertyAttribute>(false)
                                                   orderby _ ascending
                                                   select _;
                        foreach (var attribute in reevaluateAttributes)
                        {
                            viewModel.PropertyChanged += (s, e) =>
                            {
                                if (attribute.PropertyNames.Contains(e.PropertyName))
                                {
                                    var result = (bool)canExecuteMethod.Invoke(viewModel, null);
                                    selectedAppBarItem.IsEnabled = result;
                                }
                            };
                        }

                        selectedAppBarItem.IsEnabled = (bool)canExecuteMethod.Invoke(viewModel, null);
                    }

                    var actionMethod = method;
                    selectedAppBarItem.Click += (s, e) =>
                        {
                            actionMethod.Invoke(viewModel, null);
                        };
                }
            }

            page.DataContext = viewModel;

            if (typeof(IWhenReady).IsAssignableFrom(viewModel.GetType()))
            {
                (viewModel as IWhenReady).Ready();
            }
        }

        private static IEnumerable<MethodInfo> GetValidMethods(CoreData viewModel)
        {
            var validMethods = (from m in viewModel.GetType().GetMethods()
                                where !m.IsSpecialName &&
                                       m.ReturnType == typeof(void) &&
                                       m.DeclaringType != typeof(CoreData) &&
                                       !m.GetCustomAttributes<AppBarCommandAttribute>(false).Any()
                                select m).ToList();

            return validMethods;
        }

        private static void BindButton(CoreData viewModel, MethodInfo method, object control)
        {
            if (control != null && typeof(ButtonBase).IsAssignableFrom(control.GetType()))
            {
                var commandProperty = control.GetType().GetProperty("Command");
                if (commandProperty.GetValue(control) == null)
                {
                    var commandParameterProperty = control.GetType().GetProperty("CommandParameter");
                    if (commandProperty.CanWrite && commandParameterProperty.CanWrite)
                    {
                        var canExecuteExists = false;
                        var canExecuteMethod = viewModel.GetType().GetMethod("Can" + method.Name, Type.EmptyTypes);
                        if (canExecuteMethod != null)
                        {
                            canExecuteExists = canExecuteMethod.ReturnType == typeof(bool);
                        }

                        var command = new AttachedCommand(method.Name, canExecuteExists);

                        if (canExecuteMethod != null)
                        {
                            var reevaluateAttributes = from _ in canExecuteMethod.GetCustomAttributes<ReevaluatePropertyAttribute>(false)
                                                       orderby _ ascending
                                                       select _;
                            foreach (var attribute in reevaluateAttributes)
                            {
                                viewModel.PropertyChanged += (s, e) =>
                                {
                                    if (attribute.PropertyNames.Contains(e.PropertyName))
                                    {
                                        command.RaiseCanExecuteChanged();
                                    }
                                };
                            }
                        }

                        commandProperty.SetValue(control, command);
                        commandParameterProperty.SetValue(control, viewModel);
                    }
                }
            }
        }

        private static void AddTrigger(CoreData viewModel, string[] propertyNames, string methodName)
        {
            viewModel.PropertyChanged += (s, e) =>
            {
                if (propertyNames.Contains(e.PropertyName))
                {
                    var method = viewModel.GetType().GetMethod(methodName, Type.EmptyTypes);
                    if (method == null)
                    {
                        throw new Exception(string.Format(CultureInfo.CurrentCulture, "Cannot find method '{0}' - make sure it is a public method?", methodName));
                    }

                    method.Invoke(viewModel, null);
                }
            };
        }
    }
}
