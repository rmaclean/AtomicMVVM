//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

namespace AtomicMVVM
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Linq;
#if NETFX_CORE
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Core;
#else
    using System.Windows.Controls;
#endif
#if ASYNC
    using System.Threading.Tasks;
    using System.Collections.Generic;
#endif

    //todo: document and move to it's own file
    public class CoreDataLight : INotifyPropertyChanged
    {
        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }


    /// <summary>
    /// Provides the plumbing services needed in the view model for the bootstrapper to handle the binding.
    /// </summary>
    public class CoreData : INotifyPropertyChanged
    {
#if ASYNC
        private Dictionary<string, MethodDispatchMode> propertyDispatchModes;
#endif

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null && ViewControl != null)
            {
#if ASYNC
                if (propertyDispatchModes == null)
                {
                    propertyDispatchModes = new Dictionary<string, MethodDispatchMode>(this.GetType().GetRuntimeProperties().Count());
                }

                var methodDispatchMode = MethodDispatchMode.Async;
                if (propertyDispatchModes.ContainsKey(propertyName))
                {
                    methodDispatchMode = propertyDispatchModes[propertyName];
                }
                else
                {
                    var attribute = this.GetType().GetRuntimeProperty(propertyName).GetCustomAttribute<NotifyPropertyOptionsAttribute>();
                    if (attribute != null)
                    {
                        methodDispatchMode = attribute.MethodDispatchMode;
                    }
                }
#if NETFX_CORE
                if (methodDispatchMode == MethodDispatchMode.Async)
                {
                    ViewControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                    });
                }
                else
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
#endif
#if NET45
                if (methodDispatchMode == MethodDispatchMode.Async)
                { 
                ViewControl.Dispatcher.InvokeAsync(() =>
                    {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                });
                }
                else
                {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
#endif
#else
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
#endif

            }
        }

#if (ASYNC)
        /// <summary>
        /// Invokes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="System.ArgumentNullException">If the action provide is null.</exception>
        public async void Invoke(Action action)
        {
            await Task.Run(() => InvokeAsync(action));
        }
#endif

        /// <summary>
        /// Invokes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="System.ArgumentNullException">If the action provide is null.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "While it makes sense in SL & WPF, it doesn't work for Metro apps which need this to not be static.")]
#if (ASYNC)
#pragma warning disable 1998
        public async void InvokeAsync(Action action)
#pragma warning restore 1998
#else
        public void Invoke(Action action)
#endif
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

#pragma warning disable 4014
#if (NETFX_CORE)
            ViewControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
#endif
#if (NET45)
            ViewControl.Dispatcher.InvokeAsync(() =>
                {
#endif
                    action();
#if (NETFX_CORE || NET45)
                });
#endif
#pragma warning restore 4014
        }

        /// <summary>
        /// Occurs when a property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        internal UserControl ViewControl { get; set; }

        /// <summary>
        /// Occurs when view is bound.
        /// </summary>
        public event System.EventHandler OnBound;

        internal void RaiseBound()
        {
            if (OnBound != null)
            {
                OnBound(this, new EventArgs());
            }
        }

        /// <summary>
        /// Gets the bootstrapper that is being used to bind the view model.
        /// </summary>
        /// <value>
        /// The bootstrapper.
        /// </value>
        /// <seealso cref="Bootstrapper"/>
        public Bootstrapper BootStrapper { get; internal set; }
    }
}
