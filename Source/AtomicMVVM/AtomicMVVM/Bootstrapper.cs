///-----------------------------------------------------------------------
/// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
/// License: MS-PL http://www.opensource.org/licenses/MS-PL
/// Notes:
///-----------------------------------------------------------------------

namespace AtomicMVVM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;    
    using System.Globalization;
    using System.Reflection;
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
    using System.Windows;
#endif
#if SILVERLIGHT
    using System.Windows.Controls.Primitives;
#else
    using System.Windows.Input;
#endif

    /// <summary>
    /// This is the heart of AtomicMVVM - it binds everything together &amp; does the heavy lifting.
    /// </summary>
    public class Bootstrapper
    {
        private sealed class McGuffin { }
        
#if (NETFX_CORE)
        private readonly Type[] EmptyTypes = new Type[] { };
        private string ViewSuffix;
#else
        private readonly Type[] EmptyTypes = Type.EmptyTypes;
#endif
        /// <summary>
        /// The current shell that is being used to render the screen.
        /// </summary>
        /// <remarks>
        /// As of AtomicMVVM 4.1 there is no way to change the shell at runtime, this is a read-only parameter and should always match the shell passed to the start method.
        /// </remarks>
        public IShell CurrentShell { get; private set; }
        
        /// <summary>
        /// The current view model that is being used to render the screen.
        /// </summary>
        /// <remarks>
        /// This can be change by calling the ChangeView method. 
        /// </remarks>
        /// <seealso cref="ChangeView"/>
        public CoreData CurrentViewModel { get; private set; }

        /// <summary>
        /// The current view that is being used to render the screen.
        /// </summary>
        /// <remarks>
        /// This can be change by calling the ChangeView method. 
        /// </remarks>
        /// <seealso cref="ChangeView"/>
        public UserControl CurrentView { get; private set; }

        /// <summary>
        /// The list of global commands that can be bound to a button if no matching command exist in the view model.
        /// </summary>
        public List<ActionCommand> GlobalCommands { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bootstrapper" /> class.
        /// </summary>
        public Bootstrapper()
        {
            this.GlobalCommands = new List<ActionCommand>();
        }        

        /// <summary>
        /// Executes the global command.
        /// </summary>
        /// <param name="commandId">The command id to execute.</param>
        public void ExecuteGlobalCommand(string commandId)
        {
            this.GlobalCommands.Single(_ => _.Item1 == commandId).Item2();
        }       

        /// <summary>
        /// Instructs the bootstrapper to start the process of loading the shell, loading the view model &amp; and loading the view and binding them together.
        /// </summary>
        /// <param name="shell">The shell to load.</param>
        /// <param name="content">The initial view model to load.</param>
        public void Start(Type shell, Type content)
        {
            Start(shell, content, new McGuffin());
        }

        /// <summary>
        /// Instructs the bootstrapper to start the process of loading the shell, loading the view model &amp; and loading the view and binding them together.
        /// </summary>
        /// <typeparam name="TData">The type of the data to pass to the initial view model.</typeparam>
        /// <param name="shell">The type of the shell to load.</param>
        /// <param name="content">The type of the initial view model to load.</param>
        /// <param name="data">The data to pass to the initial view model.</param>
        public void Start<TData>(Type shell, Type content, TData data)
        {
            if (shell == null)
            {
                throw new ArgumentNullException("shell");
            }

            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            this.CurrentShell = shell.GetConstructor(EmptyTypes).Invoke(null) as IShell;

#if NETFX_CORE
            this.ViewSuffix = Windows.UI.ViewManagement.ApplicationView.Value.ToString();
#endif            
            this.ChangeView(content, data);
            
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

        /// <summary>
        /// Instructs the bootstrapper to start the process of loading the shell, loading the view model &amp; and loading the view and binding them together.
        /// </summary>
        /// <typeparam name="TShell">The type of the shell to load.</typeparam>
        /// <typeparam name="TContent">The type of the initial view model to load.</typeparam>
        public void Start<TShell, TContent>()
            where TShell : IShell
            where TContent : CoreData
        {
            Start(typeof(TShell), typeof(TContent));
        }

#if NETFX_CORE
        private void WindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            this.ViewSuffix = Windows.UI.ViewManagement.ApplicationView.Value.ToString();
            ChangeView();
        }
#endif

        /// <summary>
        /// Instructs the bootstrapper to start the process of loading the shell, loading the view model &amp; and loading the view and binding them together.
        /// </summary>
        /// <typeparam name="TShell">The type of the shell to load.</typeparam>
        /// <typeparam name="TContent">The type of the initial view model to load.</typeparam>
        /// <typeparam name="TData">The type of the data to pass to the initial view model.</typeparam>
        /// <param name="data">The data to pass to the initial view model.</param>        
        public void Start<TShell, TContent, TData>(TData data)
            where TContent : CoreData
            where TShell : IShell
        {
            Start(typeof(TShell), typeof(TContent), data);    
        }

        /// <summary>
        /// Changes the view model and the associated view.
        /// </summary>
        /// <typeparam name="TData">The type of the data to pass to the view model.</typeparam>
        /// <param name="newContent">The type of the new view model to load.</param>
        /// <param name="data">The data to pass to the view model.</param>
        public void ChangeView<TData>(Type newContent, TData data)
        {
            if (newContent == null)
            {
                throw new ArgumentNullException("newContent");
            }

            if (typeof(TData) == typeof(McGuffin))
            {
                CurrentViewModel = newContent.GetConstructor(EmptyTypes).Invoke(null) as CoreData;
            }
            else
            {
                CurrentViewModel = newContent.GetConstructor(new[] { typeof(TData) }).Invoke(new object[] { data }) as CoreData;
            }

            CurrentViewModel.BootStrapper = this;
            ChangeView();
        }

        /// <summary>
        /// Changes the view model and the associated view.
        /// </summary>
        /// <param name="newContent">The type of the new view model to load.</param>
        public void ChangeView(Type newContent)
        {
            ChangeView(newContent, new McGuffin());
        }

        /// <summary>
        /// Changes the view model and the associated view.
        /// </summary>
        /// <typeparam name="TNewContent">The type of the new view model to load.</typeparam>
        /// <typeparam name="TData">The type of the data to pass to the view model.</typeparam>
        /// <param name="data">The data to pass to the view model.</param>
        public void ChangeView<TNewContent, TData>(TData data)
            where TNewContent : CoreData
        {
            ChangeView(typeof(TNewContent), data);
        }

        /// <summary>
        /// Changes the view model and the associated view.
        /// </summary>
        /// <typeparam name="TNewContent">The type of the new view model to load.</typeparam>
        public void ChangeView<TNewContent>()
            where TNewContent : CoreData
        {
            ChangeView(typeof(TNewContent), new McGuffin());
        }

        private Type GetView(bool withSuffix)
        {
            var viewNamespace = ".Views.";
#if NETFX_CORE
            if (withSuffix && !string.IsNullOrWhiteSpace(this.ViewSuffix))
            {
                viewNamespace += this.ViewSuffix + ".";
            }
#endif

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

#if NETFX_CORE
            var validMethods = (from m in CurrentViewModel.GetType().GetRuntimeMethods()
#else
            var validMethods = (from m in CurrentViewModel.GetType().GetMethods()
#endif
                                where !m.IsSpecialName &&
                                      m.DeclaringType != typeof(CoreData) &&
                                      m.ReturnType == typeof(void)
                                select m).ToList();

            BindMethods(validMethods);

            BindGlobalCommands();

            CurrentViewModel.RaiseBound();
            CurrentView.DataContext = CurrentViewModel;
            CurrentShell.ChangeContent(CurrentView);
        }

        private void BindMethods(List<MethodInfo> validMethods)
        {
            foreach (var method in validMethods)
            {
                var attributes = from _ in method.GetCustomAttributes<TriggerPropertyAttribute>(false)
                                 orderby _.Priority ascending
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification="It is the parameter the user passes in - however it is not from the anonymous method that it is in")]
        private void AddTrigger(IEnumerable<string> propertyNames, string methodName)
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
                            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Cannot find method '{0}' - make sure it is a public method?", methodName), "methodName");
                        }

                        method.Invoke(CurrentViewModel, null);
                    }
                };
        }
    }              
}
