
namespace AtomicMVVM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
#if WINRT
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Core;
    using Windows.UI.Xaml.Data;
#else
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Controls.Primitives;
    using System.ComponentModel;
#endif

    public class Bootstrapper<TShell, TContent>
        where TShell : IShell
        where TContent : CoreData
    {
#if (WINRT)
        private readonly Type[] EmptyTypes = new Type[] { };
#else
        private readonly Type[] EmptyTypes = Type.EmptyTypes;
#endif
        private IShell shell;
        private CoreData viewModel;
        private UserControl view;
        public List<Tuple<string, Action>> GlobalCommands { get; private set; }

        public Bootstrapper()
        {
            this.GlobalCommands = new List<Tuple<string, Action>>();

            if (shell == null)
            {
#if (WINRT)
                Window.Current.Dispatcher.Invoke(CoreDispatcherPriority.High, (s, e) =>
                    {
#endif
                shell = (IShell)typeof(TShell).GetConstructor(EmptyTypes).Invoke(null);
#if (WINRT)
                    }, this, null);
#endif
            }

            this.ChangeView<TContent>();

#if WINRT
             Window.Current.Content = shell as UIElement;
            Window.Current.Activate();
#else
#if SILVERLIGHT
             Application.Current.RootVisual = shell as UIElement;
#else
            (shell as Window).Show();
#endif
#endif
        }

        public void ChangeView<TNewContent, TData>(TData data)
            where TNewContent : CoreData
        {
            viewModel = typeof(TNewContent).GetConstructor(new[] { typeof(TData) }).Invoke(new object[] { data }) as CoreData;
            ChangeView();
        }

        public void ChangeView<TNewContent>()
            where TNewContent : CoreData
        {
            viewModel = typeof(TNewContent).GetConstructor(EmptyTypes).Invoke(null) as CoreData;
            ChangeView();
        }

        private void ChangeView()
        {
            var viewName = viewModel.GetType().AssemblyQualifiedName.Replace(".ViewModels.", ".Views.");

#if WINRT
            var viewType = Type.GetType(viewName);
#else
            var viewType = Type.GetType(viewName, true, true);
#endif
            view = viewType.GetConstructor(EmptyTypes).Invoke(null) as UserControl;

            this.viewModel.ViewControl = view;

            var validMethods = (from m in viewModel.GetType().GetMethods()
                                where m.IsPublic &&
                                     !m.IsSpecialName &&
#if !WINRT
 !m.Attributes.HasFlag(MethodAttributes.VtableLayoutMask) &&
#endif
 m.ReturnType == typeof(void)
                                select m).ToList();

            foreach (var method in validMethods)
            {
                foreach (var attribute in method.GetCustomAttributes<TriggerPropertyAttribute>(false))
                {
                    AddTrigger(attribute.PropertyName, method.Name);
                }

                var control = view.FindName(method.Name);
#if WINRT
                if (control != null && typeof(ButtonBase).GetTypeInfo().IsAssignableFrom(control.GetType().GetTypeInfo()))
#else
#if SILVERLIGHT
                if (control != null && typeof(ButtonBase).IsAssignableFrom(control.GetType()))
#else
                if (control != null && control is ICommandSource)
#endif
#endif
                {
                    var commandProperty = control.GetType().GetProperty("Command");
                    if (commandProperty.GetValue(control) == null)
                    {
                        var commandParameterProperty = control.GetType().GetProperty("CommandParameter");
                        if (commandProperty.CanWrite && commandParameterProperty.CanWrite)
                        {
                            var canExecuteExists = false;
                            var canExecuteMethod = viewModel.GetType().GetMethod("Can" + method.Name, EmptyTypes);
                            if (canExecuteMethod != null)
                            {
                                canExecuteExists = canExecuteMethod.ReturnType == typeof(bool);
                            }

                            var command = new AttachedCommand(method.Name, canExecuteExists);

                            if (canExecuteMethod != null)
                            {
                                foreach (var attribute in canExecuteMethod.GetCustomAttributes<ReevaluatePropertyAttribute>(false))
                                {
                                    viewModel.PropertyChanged += (s, e) =>
                                    {
                                        if (e.PropertyName == attribute.PropertyName)
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

            foreach (var method in GlobalCommands)
            {
                var control = view.FindName(method.Item1);
#if WINRT
                if (control != null && typeof(ButtonBase).GetTypeInfo().IsAssignableFrom(control.GetType().GetTypeInfo()))
#else
#if SILVERLIGHT
                if (control != null && typeof(ButtonBase).IsAssignableFrom(control.GetType()))
#else
                if (control != null && control is ICommandSource)
#endif
#endif
                {
                    var commandProperty = control.GetType().GetProperty("Command");
                    if (commandProperty.GetValue(control) == null)
                    {
                        var commandParameterProperty = control.GetType().GetProperty("CommandParameter");
                        if (commandProperty.CanWrite && commandParameterProperty.CanWrite)
                        {
                            var command = new GlobalCommand(method.Item2);

                            commandProperty.SetValue(control, command);
                            commandParameterProperty.SetValue(control, viewModel);
                        }
                    }
                }
            }

            view.DataContext = viewModel;

            shell.ChangeContent(view);
        }

        private void AddTrigger(string propertyName, string methodName)
        {
            this.viewModel.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == propertyName)
                    {
                        viewModel.GetType().GetMethod(methodName).Invoke(viewModel, null);
                    }
                };
        }
    }

    public class GlobalCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

#if (WINRT)
        public event Windows.UI.Xaml.EventHandler CanExecuteChanged;
#else
        public event EventHandler CanExecuteChanged;
#endif
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


    public class AttachedCommand : ICommand
    {
#if (WINRT)
        private readonly Type[] EmptyTypes = new Type[] { };
#else
        private readonly Type[] EmptyTypes = Type.EmptyTypes;
#endif
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
                canExecuteMethod = parameter.GetType().GetMethod("Can" + this.methodName, EmptyTypes);
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

#if (WINRT)
        public event Windows.UI.Xaml.EventHandler CanExecuteChanged;
#else
        public event EventHandler CanExecuteChanged;
#endif
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
                executeMethod = parameter.GetType().GetMethod(this.methodName);
            }

            executeMethod.Invoke(parameter, null);
        }
    }


    public class CoreData : INotifyPropertyChanged
    {
        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                this.Invoke(() => PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));
            }
        }

        public void Invoke(Action action)
        {
#if (WINRT)
            ViewControl.Dispatcher.Invoke(CoreDispatcherPriority.Normal, (s, e) =>
                {
#endif
            action();
#if (WINRT)
                }, this, null);
#endif
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public UserControl ViewControl { get; set; }
    }

    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class ReevaluatePropertyAttribute : System.Attribute
    {
        public ReevaluatePropertyAttribute(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        public string PropertyName { get; private set; }
    }

    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class TriggerPropertyAttribute : System.Attribute
    {
        public TriggerPropertyAttribute(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        public string PropertyName { get; private set; }
    }

    public interface IShell
    {
        void ChangeContent(UserControl viewContent);
    }
}
