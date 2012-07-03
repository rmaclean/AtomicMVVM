
namespace AtomicMVVM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Input;
    using System.ComponentModel;
    using System.Globalization;
#if WINDOWS_PHONE
    using ActionCommand = Tuple<string, System.Action>;
#else
    using ActionCommand = System.Tuple<string, System.Action>;
#endif
#if NETFX_CORE
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml;
    using Windows.UI.Core;
    using Windows.UI.Xaml.Controls.Primitives;
#else
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Navigation;
#endif

    public class Bootstrapper
    {
#if (NETFX_CORE)
        private readonly Type[] EmptyTypes = new Type[] { };
        private string ViewSuffix;
#else
        private readonly Type[] EmptyTypes = Type.EmptyTypes;
#endif
        public IShell CurrentShell { get; private set; }
        public CoreData CurrentViewModel { get; private set; }
        public UserControl CurrentView { get; private set; }
        public List<ActionCommand> GlobalCommands { get; private set; }

        public Bootstrapper()
        {
            this.GlobalCommands = new List<ActionCommand>();
        }

        private class McGuffin { }

        public void Start<TShell, TContent>()
            where TShell : IShell
            where TContent : CoreData
        {
            Start<TShell, TContent, McGuffin>(null);
        }

#if NETFX_CORE
        private void WindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            this.ViewSuffix = Windows.UI.ViewManagement.ApplicationView.Value.ToString();
            ChangeView();
        }
#endif

        public void Start<TShell, TContent, TData>(TData data)
            where TContent : CoreData
            where TShell : IShell
        {
            CurrentShell = (IShell)typeof(TShell).GetConstructor(EmptyTypes).Invoke(null);
#if NETFX_CORE
            this.ViewSuffix = Windows.UI.ViewManagement.ApplicationView.Value.ToString();
#endif
            if (typeof(TData) == typeof(McGuffin))
            {                
                this.ChangeView<TContent>();
            }
            else
            {
                this.ChangeView<TContent, TData>(data);
            }

#if NETFX_CORE
            var uiShell = CurrentShell as UIElement;
            if (uiShell != null)
            {
                Window.Current.Content = uiShell;
                Window.Current.SizeChanged += WindowSizeChanged;
                Window.Current.Activate();
            }
#else
#if SILVERLIGHT
            var uiShell = CurrentShell as UIElement;
            if (uiShell != null)
            {
                Application.Current.RootVisual = uiShell; ;
            }
#else
            var window = CurrentShell as Window;
            if (window != null)
            {
                window.Show();
            }
#endif
#endif
        }

        public void ChangeView<TNewContent, TData>(TData data)
            where TNewContent : CoreData
        {
            CurrentViewModel = typeof(TNewContent).GetConstructor(new[] { typeof(TData) }).Invoke(new object[] { data }) as CoreData;
            CurrentViewModel.BootStrapper = this;
            ChangeView();
        }

        public void ChangeView<TNewContent>()
            where TNewContent : CoreData
        {
            CurrentViewModel = typeof(TNewContent).GetConstructor(EmptyTypes).Invoke(null) as CoreData;
            CurrentViewModel.BootStrapper = this;
            ChangeView();
        }

        private Type GetView(bool withSuffix)
        {
            var viewNamespace = ".Views.";
            if (withSuffix && !string.IsNullOrWhiteSpace(this.ViewSuffix))
            {
                viewNamespace += this.ViewSuffix + ".";
            }

            var viewName = CurrentViewModel.GetType().AssemblyQualifiedName.Replace(".ViewModels.", viewNamespace);

#if (NETFX_CORE || WINDOWS_PHONE)
            var viewType = Type.GetType(viewName);
#else
            var viewType = Type.GetType(viewName, true, true);
#endif
            if (viewType == null && withSuffix)
            {
                return GetView(false);
            }

            return viewType;
        }

        private void ChangeView()
        {
#if NETFX_CORE
            var viewType = GetView(true);
            if (CurrentView != null && viewType == CurrentView.GetType())
            {
                return;
            }
#else
            var viewType = GetView(false);
#endif
            CurrentView = viewType.GetConstructor(EmptyTypes).Invoke(null) as UserControl;

            this.CurrentViewModel.ViewControl = CurrentView;

#if NETFX_CORE
            var validMethods = (from m in CurrentViewModel.GetType().GetRuntimeMethods()
#else
            var validMethods = (from m in CurrentViewModel.GetType().GetMethods()
#endif
                                where !m.IsSpecialName &&
                                      m.DeclaringType != typeof(CoreData) &&
                                      m.ReturnType == typeof(void)
                                select m).ToList();

            foreach (var method in validMethods)
            {
                var attributes = from _ in method.GetCustomAttributes<TriggerPropertyAttribute>(false)
                                 orderby _.Order ascending
                                 select _;
                foreach (var attribute in attributes)
                {
                    AddTrigger(attribute.PropertyNames, method.Name);
                }

                var control = CurrentView.FindName(method.Name);
#if NETFX_CORE
                if (control != null && typeof(ButtonBase).GetTypeInfo().IsAssignableFrom(control.GetType().GetTypeInfo()))
#else
#if SILVERLIGHT
                if (control != null && typeof(ButtonBase).IsAssignableFrom(control.GetType()))
#else
                if (control != null && control is ICommandSource)
#endif
#endif

#if NETFX_CORE
                {
                    var commandProperty = typeof(ButtonBase).GetRuntimeProperty("Command");
#else
                {
                    var commandProperty = control.GetType().GetProperty("Command");
#endif
                    if (commandProperty.GetValue(control) == null)
                    {
#if NETFX_CORE
                        var commandParameterProperty = typeof(ButtonBase).GetRuntimeProperty("CommandParameter");
#else
                        var commandParameterProperty = control.GetType().GetProperty("CommandParameter");
#endif
                        if (commandProperty.CanWrite && commandParameterProperty.CanWrite)
                        {
                            var canExecuteExists = false;
#if NETFX_CORE
                            var canExecuteMethod = CurrentViewModel.GetType().GetRuntimeMethod("Can" + method.Name, EmptyTypes);
#else
                            var canExecuteMethod = CurrentViewModel.GetType().GetMethod("Can" + method.Name, EmptyTypes);
#endif
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
                                    CurrentViewModel.PropertyChanged += (s, e) =>
                                    {
                                        if (attribute.PropertyNames.Contains(e.PropertyName))
                                        {
                                            command.RaiseCanExecuteChanged();
                                        }
                                    };
                                }
                            }

                            commandProperty.SetValue(control, command);
                            commandParameterProperty.SetValue(control, CurrentViewModel);
                        }
                    }
                }
            }

            BindGlobalCommands();

            CurrentViewModel.RaiseBound();
            CurrentView.DataContext = CurrentViewModel;
            CurrentShell.ChangeContent(CurrentView);
        }

#if NETFX_CORE || SILVERLIGHT
        internal void BindGlobalCommands(Control view = null, List<ActionCommand> commands = null)
#else
        internal void BindGlobalCommands(ContentControl view = null, List<ActionCommand> commands = null)
#endif
        {
            if (view == null)
            {
                view = this.CurrentView;
            }

            if (commands == null)
            {
                commands = this.GlobalCommands;
            }

            foreach (var method in commands)
            {
                var control = view.FindName(method.Item1);
#if NETFX_CORE
                if (control != null && typeof(ButtonBase).GetTypeInfo().IsAssignableFrom(control.GetType().GetTypeInfo()))
#else
#if SILVERLIGHT
                if (control != null && typeof(ButtonBase).IsAssignableFrom(control.GetType()))
#else
                if (control != null && control is ICommandSource)
#endif
#endif
                {
#if NETFX_CORE
                    var commandProperty = typeof(ButtonBase).GetRuntimeProperty("Command");
#else
                    var commandProperty = control.GetType().GetProperty("Command");
#endif
                    if (commandProperty.GetValue(control) == null)
                    {
#if NETFX_CORE
                        var commandParameterProperty = typeof(ButtonBase).GetRuntimeProperty("CommandParameter");
#else
                        var commandParameterProperty = control.GetType().GetProperty("CommandParameter");
#endif
                        if (commandProperty.CanWrite && commandParameterProperty.CanWrite)
                        {
                            var command = new GlobalCommand(method.Item2);

                            commandProperty.SetValue(control, command);
                            commandParameterProperty.SetValue(control, CurrentViewModel);
                        }
                    }
                }
            }
        }

        private void AddTrigger(string[] propertyNames, string methodName)
        {
            this.CurrentViewModel.PropertyChanged += (s, e) =>
                {
                    if (propertyNames.Contains(e.PropertyName))
                    {
#if NETFX_CORE
                        var method = CurrentViewModel.GetType().GetRuntimeMethod(methodName, EmptyTypes);
#else
                        var method = CurrentViewModel.GetType().GetMethod(methodName, EmptyTypes);
#endif
                        if (method == null)
                        {
                            throw new Exception(string.Format(CultureInfo.CurrentCulture, "Cannot find method '{0}' - make sure it is a public method?", methodName));
                        }

                        method.Invoke(CurrentViewModel, null);
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
#if (NETFX_CORE)
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
#if NETFX_CORE
                canExecuteMethod = parameter.GetType().GetRuntimeMethod("Can" + this.methodName, EmptyTypes);
#else
                canExecuteMethod = parameter.GetType().GetMethod("Can" + this.methodName, EmptyTypes);
#endif
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
#if NETFX_CORE
                executeMethod = parameter.GetType().GetRuntimeMethod(this.methodName, EmptyTypes);
#else
                executeMethod = parameter.GetType().GetMethod(this.methodName, EmptyTypes);
#endif
            }

            executeMethod.Invoke(parameter, null);
        }
    }


    public class CoreData : INotifyPropertyChanged
    {
        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null && ViewControl != null)
            {
#if (NETFX_CORE)
                ViewControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
#endif
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
#if (NETFX_CORE)
                });
#endif
            }
        }

        public void Invoke(Action action)
        {
#if (NETFX_CORE)
            ViewControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
#endif
                    action();
#if (NETFX_CORE)
                });
#endif
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public UserControl ViewControl { get; set; }

        public event System.EventHandler OnBound;

        public void RaiseBound()
        {
            if (OnBound != null)
            {
                OnBound(this, new EventArgs());
            }
        }

        public Bootstrapper BootStrapper { get; set; }
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

    public static class IShellExtensions
    {
        public static void BindGlobalCommands(this ContentControl shell, Bootstrapper bootStrapper)
        {
            bootStrapper.BindGlobalCommands(shell, bootStrapper.GlobalCommands);
        }
    }

    public interface IShell
    {
        void ChangeContent(UserControl viewContent);
    }

    public class StubShell : IShell
    {
        public void ChangeContent(UserControl viewContent)
        {
            // sure thing :)
        }
    }
}
