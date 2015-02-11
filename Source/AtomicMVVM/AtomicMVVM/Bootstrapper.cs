//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

namespace AtomicMVVM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Globalization;
    using System.Reflection;
    using ActionCommand = System.Tuple<string, System.Action>;
#if WINRT
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml;
    using Windows.UI.Core;
    using Windows.UI.Xaml.Controls.Primitives;
#else
    using System.Windows.Controls;
    using System.Windows;
    using System.Windows.Input;
    using System.Diagnostics;
    using System.Windows.Controls.Primitives;
#endif

    /// <summary>
    /// This is the heart of AtomicMVVM - it binds everything together &amp; does the heavy lifting.
    /// </summary>
    public class Bootstrapper
    {
        private Stack<Tuple<Type, object>> BackStack = new Stack<Tuple<Type, object>>();
        private sealed class McGuffin { }
        private List<object> CurrentViewExtendedChildControls;
#if WINRT
        private SortedDictionary<string, SortedDictionary<int, Type>> knownLandscapeViews;
        private SortedDictionary<string, SortedDictionary<int, Type>> knownPortraitViews;
        private readonly Type[] EmptyTypes = new Type[] { };
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
        /// <seealso cref="ChangeView(System.Type)"/>
        public CoreData CurrentViewModel { get; private set; }

        /// <summary>
        /// The current view that is being used to render the screen.
        /// </summary>
        /// <remarks>
        /// This can be change by calling the ChangeView method. 
        /// </remarks>
        /// <seealso cref="ChangeView(System.Type)"/>
        public UserControl CurrentView { get; private set; }

        /// <summary>
        /// The list of global commands that can be bound to a button if no matching command exist in the view model.
        /// </summary>
        public List<ActionCommand> GlobalCommands { get; private set; }

        /// <summary>
        /// Is raised after the start has completed (i.e. when we have the first screen up).
        /// </summary>
        public event Action AfterStartCompletes;
        private List<MethodInfo> validMethods;

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

#if WINRT
        /// <summary>
        /// Inspects the assembly for all views and preloads them into the collections for portraits &amp; landscape
        /// </summary>
        private void LoadAllViewInformation()
        {
            var landscapeViews = new SortedDictionary<string, SortedDictionary<int, Type>>();
            var portraitViews = new SortedDictionary<string, SortedDictionary<int, Type>>();

            var allViews = from type in this.CurrentShell.GetType().GetTypeInfo().Assembly.ExportedTypes
                           where type.GetTypeInfo().IsSubclassOf(typeof(UserControl)) &&
                           type.Namespace.IndexOf(".Views", StringComparison.OrdinalIgnoreCase) > 0
                           select type;

            var stringWidth = "";
            var maxWidth = 0;
            var portraitOnlyView = false;
            var orientation = "";
            var lastDotInNamespace = 0;
            foreach (var view in allViews)
            {
                portraitOnlyView = false;

                if (!landscapeViews.ContainsKey(view.Name))
                {
                    landscapeViews.Add(view.Name, new SortedDictionary<int, Type>());
                    portraitViews.Add(view.Name, new SortedDictionary<int, Type>());
                }

                lastDotInNamespace = view.Namespace.LastIndexOf('.');
                orientation = view.Namespace.Substring(0, lastDotInNamespace);
                portraitOnlyView = orientation.EndsWith("Portrait", StringComparison.OrdinalIgnoreCase);

                stringWidth = view.Namespace.Substring(lastDotInNamespace + 2);
                maxWidth = Int32.MaxValue;

                try
                {
                    maxWidth = Convert.ToInt32(stringWidth);
                }
                catch (FormatException)
                {
                    //do nothing
                }

                if (portraitOnlyView)
                {
                    portraitViews[view.Name].Add(maxWidth, view);
                }
                else
                {
                    landscapeViews[view.Name].Add(maxWidth, view);
                    portraitViews[view.Name].Add(maxWidth, view);
                }
            }

            this.knownLandscapeViews = landscapeViews;
            this.knownPortraitViews = portraitViews;
        }
#endif

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

#if WINRT
            LoadAllViewInformation();
#endif
            this.ChangeView(content, data);

#if WINRT
            var uiShell = CurrentShell as UIElement;
            if (uiShell != null)
            {
                Window.Current.Content = uiShell;
                Window.Current.SizeChanged += WindowSizeChanged;
                Window.Current.Activate();
            }
#else
            var window = CurrentShell as Window;
            if (window != null)
            {
                window.Show();
            }
#endif
            if (AfterStartCompletes != null)
            {
                AfterStartCompletes();
            }
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

#if WINRT
        private void WindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            ChangeView(true);
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
        /// <param name="addToBackStack">Should the new page be added to the back stack.</param>
        public void ChangeView<TData>(Type newContent, TData data, bool addToBackStack = true)
        {
            if (newContent == null)
            {
                throw new ArgumentNullException("newContent");
            }

            if (addToBackStack)
            {
                BackStack.Push(Tuple.Create(newContent, (object)data));
            }

            if (data.GetType() == typeof(McGuffin))
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
        /// <param name="addToBackStack">Should the new page be added to the back stack.</param>
        public void ChangeView(Type newContent, bool addToBackStack = true)
        {
            ChangeView(newContent, new McGuffin());
        }

        /// <summary>
        /// Changes the view model and the associated view.
        /// </summary>
        /// <typeparam name="TNewContent">The type of the new view model to load.</typeparam>
        /// <typeparam name="TData">The type of the data to pass to the view model.</typeparam>
        /// <param name="data">The data to pass to the view model.</param>
        /// <param name="addToBackStack">Should the new page be added to the back stack.</param>
        public void ChangeView<TNewContent, TData>(TData data, bool addToBackStack = true)
            where TNewContent : CoreData
        {
            ChangeView(typeof(TNewContent), data);
        }

        /// <summary>
        /// Changes the view model and the associated view.
        /// </summary>
        /// <typeparam name="TNewContent">The type of the new view model to load.</typeparam>
        /// <param name="addToBackStack">Should the new page be added to the back stack.</param>
        public void ChangeView<TNewContent>(bool addToBackStack = true)
            where TNewContent : CoreData
        {
            ChangeView(typeof(TNewContent), new McGuffin());
        }

        /// <summary>
        /// Allows you to go back to the previous page
        /// </summary>
        public void GoBack()
        {
            BackStack.Pop(); // remove current page
            var previousPage = BackStack.Pop();
            ChangeView(previousPage.Item1, previousPage.Item2);
        }

#if !WINRT
        private Type GetView()
        {
            var viewNamespace = ".Views.";
            var viewName = CurrentViewModel.GetType().AssemblyQualifiedName.Replace(".ViewModels.", viewNamespace);
            var viewType = Type.GetType(viewName, true, true);
            if (viewType == null)
            {
                return GetView();
            }

            return viewType;
        }

#endif

#if WINRT
        private Type GetView()
        {
            var availableViews = this.knownLandscapeViews;
            var requestedWidth = Window.Current.Bounds.Width;
            switch (Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().Orientation)
            {
                case Windows.UI.ViewManagement.ApplicationViewOrientation.Landscape:
                    {
                        //no need to do anything
                        break;
                    }
                case Windows.UI.ViewManagement.ApplicationViewOrientation.Portrait:
                    {
                        availableViews = this.knownPortraitViews;
                        break;
                    }
            }

            var viewModelName = this.CurrentViewModel.GetType().Name;
            foreach (var viewSet in availableViews)
            {
                if (viewSet.Key == viewModelName)
                {
                    var views = viewSet.Value;
                    foreach (var view in views)
                    {
                        if (requestedWidth <= view.Key)
                        {
                            return view.Value;
                        }
                    }
                }
            }

            throw new Exception("Requested view cannot be found");
        }
#endif
        private void ChangeView(bool forceChange = false)
        {
            var viewType = default(Type);
            viewType = GetView();
#if WINRT
            if (!forceChange)
            {
                if (CurrentView != null && viewType == CurrentView.GetType())
                {
                    return;
                }
            }
#endif
            CurrentView = viewType.GetConstructor(EmptyTypes).Invoke(null) as UserControl;

#if WINRT
            this.validMethods = (from m in CurrentViewModel.GetType().GetRuntimeMethods()
#else
            this.validMethods = (from m in CurrentViewModel.GetType().GetMethods()
#endif
                                 where !m.IsSpecialName &&
                                      m.DeclaringType != typeof(CoreData) &&
                                      m.ReturnType == typeof(void)
                                 select m).ToList();

            CurrentViewModel.ViewControl = CurrentView;

            CurrentView.DataContext = CurrentViewModel;

            var initializedComponent = CurrentView.GetType().GetRuntimeMethod("InitializeComponent", EmptyTypes);
            initializedComponent.Invoke(this.CurrentView, null);

            this.CurrentViewExtendedChildControls = UIExtendedControls();

            BindMethods(this.CurrentView, validMethods);

            BindGlobalCommands();

            CurrentViewModel.RaiseBound();

            var whenDataBound = CurrentView as IWhenDataBound;
            if (whenDataBound != null)
            {
                whenDataBound.DataContextBound(CurrentViewModel);
            }

            CurrentShell.ChangeContent(CurrentView);
        }

        private List<object> UIExtendedControls()
        {
            var result = new List<object>();

            GetChildControls(CurrentView, result);

            return result;
        }

        /// <summary>
        /// Used to bind commands with the provided UIObject
        /// </summary>
        /// <param name="uiObject">The UIObject to traverse to see if it has ICommands compatible items that can be attached.</param>
        /// <remarks>At this stage this is tentative and maybe removed by release of 5.0</remarks>
        public void UpdateCommands(FrameworkElement uiObject)
        {
            var result = new List<object>();

            GetChildControls(uiObject, result);
            BindMethods(uiObject, validMethods);
        }

        private void GetChildControls(DependencyObject uiObject, List<object> result)
        {
            if (uiObject == null)
            {
                return;
            }

            var buttonBase = uiObject as ButtonBase;
            if (buttonBase != null)
            {
                var o = buttonBase.GetValue(UIExtensions.CommandNameProperty);
                if (o != null)
                {
                    result.Add(buttonBase);
                }
            }

            var userControl = uiObject as UserControl;
            if (userControl != null)
            {
#if NET45
                var content = userControl.Content as DependencyObject;
                GetChildControls(content, result);
#else
                GetChildControls(userControl.Content, result);
#endif
            }

            var panel = uiObject as Panel;
            if (panel != null)
            {
                foreach (var item in panel.Children)
                {
#if NET45
                    var childItem = item as DependencyObject;
                    GetChildControls(childItem, result);
#else
                    GetChildControls(item, result);
#endif

                }
            }

            var contentControl = uiObject as ContentControl;
            if (contentControl != null)
            {
                var depObject = contentControl.Content as DependencyObject;
                if (depObject != null)
                {
                    GetChildControls(depObject, result);
                }
            }

            var itemsControl = uiObject as ItemsControl;
            if (itemsControl != null)
            {
                foreach (var item in itemsControl.Items)
                {
                    var depObject = item as DependencyObject;
                    if (depObject != null)
                    {
                        GetChildControls(depObject, result);
                    }
                }
            }
        }

        private List<object> FindControl(FrameworkElement uiObject, string methodName)
        {
            var result = new List<object>();
            var nameMatch = uiObject.FindName(methodName);
            if (nameMatch != null)
            {
                result.Add(nameMatch);
            }

            foreach (ButtonBase item in this.CurrentViewExtendedChildControls)
            {
                var extendedName = (string)item.GetValue(UIExtensions.CommandNameProperty);
                if (extendedName == methodName)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        private void BindMethods(FrameworkElement uiObject, List<MethodInfo> validMethods)
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

                var controls = FindControl(uiObject, method.Name);
                foreach (var item in controls)
                {
                    BindControl(method, item);
                }
            }
        }

        private void BindControl(MethodInfo method, object control)
        {

#if WINRT
            if (control != null && typeof(ButtonBase).GetTypeInfo().IsAssignableFrom(control.GetType().GetTypeInfo()))
#else
                if (control != null && control is ICommandSource)
#endif

#if WINRT
            {
                var commandProperty = typeof(ButtonBase).GetRuntimeProperty("Command");
#else
                {
                    var commandProperty = control.GetType().GetProperty("Command");
#endif
                if (commandProperty.GetValue(control) == null)
                {
#if WINRT
                    var commandParameterProperty = typeof(ButtonBase).GetRuntimeProperty("CommandParameter");
#else
                        var commandParameterProperty = control.GetType().GetProperty("CommandParameter");
#endif
                    if (commandProperty.CanWrite && commandParameterProperty.CanWrite)
                    {
                        var canExecuteExists = false;
#if WINRT
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

#if WINRT
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

#if WINRT
                if (control != null && typeof(ButtonBase).GetTypeInfo().IsAssignableFrom(control.GetType().GetTypeInfo()))
#else
                if (control != null && control is ICommandSource)
#endif
                {
#if WINRT
                    var commandProperty = typeof(ButtonBase).GetRuntimeProperty("Command");
#else
                    var commandProperty = control.GetType().GetProperty("Command");
#endif
                    if (commandProperty.GetValue(control) == null)
                    {
#if WINRT
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "It is the parameter the user passes in - however it is not from the anonymous method that it is in")]
        private void AddTrigger(IEnumerable<string> propertyNames, string methodName)
        {
            this.CurrentViewModel.PropertyChanged += (s, e) =>
                {
                    if (propertyNames.Contains(e.PropertyName))
                    {
#if WINRT
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
