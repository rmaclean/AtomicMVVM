﻿
namespace AtomicPhoneMVVM
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;

    public static class Bootstrapper
    {
        public static void BindData<T>(this PhoneApplicationPage page)
        {
            BindData<T, object>(page, null);
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

            var validMethods = (from m in viewModel.GetType().GetMethods()
                                where !m.IsSpecialName &&
                                       m.ReturnType == typeof(void) &&
                                       m.DeclaringType != typeof(CoreData) &&
                                       !m.GetCustomAttributes<AppBarCommandAttribute>(false).Any()
                                select m).ToList();

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

                    selectedAppBarItem.Click += (s,e) =>
                        {
                            method.Invoke(viewModel, null);
                        };
                }
            }

            page.DataContext = viewModel;
            if (typeof(IWhenReady).IsAssignableFrom(viewModel.GetType()))
            {
                (viewModel as IWhenReady).Ready();
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

    class GlobalCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67

        private Action action;

        public GlobalCommand(Action action)
        {
            this.action = action;
        }

        public void Execute(object parameter)
        {
            action();
        }
    }

    class AttachedCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            if (!canExecuteExists)
            {
                return true;
            }

            if (parameter == null)
            {
                return false;
            }

            if (canExecuteMethod == null)
            {
                canExecuteMethod = parameter.GetType().GetMethod("Can" + this.methodName, Type.EmptyTypes);
            }

            return (bool)canExecuteMethod.Invoke(parameter, null);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, new EventArgs());
            }
        }

        public event EventHandler CanExecuteChanged;
        private string methodName;
        private MethodInfo executeMethod;
        private MethodInfo canExecuteMethod;
        private bool canExecuteExists;

        public AttachedCommand(string methodName, bool canExecuteExists)
        {
            this.methodName = methodName;
            this.canExecuteExists = canExecuteExists;
        }

        public void Execute(object parameter)
        {
            if (executeMethod == null)
            {
                executeMethod = parameter.GetType().GetMethod(this.methodName, Type.EmptyTypes);
            }

            executeMethod.Invoke(parameter, null);
        }
    }


    public interface IPushMessage
    {
        void Push(string message);
    }

    public interface IWhenReady
    {
        void Ready();
    }

    public class CoreData : INotifyPropertyChanged
    {
        public IPushMessage Page { get; set; }

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Navigate(string page)
        {
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(page, UriKind.Relative));
        }

        public void PushMessage(string message)
        {
            if (Page != null)
            {
                Page.Push(message);
            }
        }
    }

    [AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ReevaluatePropertyAttribute : System.Attribute
    {
        public ReevaluatePropertyAttribute(params string[] propertyNames)
            : this(0, propertyNames)
        {
        }

        public ReevaluatePropertyAttribute(int order, params string[] propertyNames)
        {
            this.PropertyNames = propertyNames;
            this.Order = order;
        }

        public string[] PropertyNames { get; private set; }
        public int Order { get; private set; }
    }

    [AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class TriggerPropertyAttribute : System.Attribute
    {
        public TriggerPropertyAttribute(params string[] propertyNames)
            : this(0, propertyNames)
        {
        }

        public TriggerPropertyAttribute(int order, params string[] propertyNames)
        {
            this.PropertyNames = propertyNames;
            this.Order = order;
        }

        public string[] PropertyNames { get; private set; }
        public int Order { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class AppBarCommandAttribute : Attribute
    {
        public string AppBarText { get; private set; }
        public AppBarCommandAttribute(string appBarText)
        {
            this.AppBarText = appBarText;
        }
    }
}