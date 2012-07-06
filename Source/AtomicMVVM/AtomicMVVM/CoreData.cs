
namespace AtomicMVVM
{
    using System;
    using System.ComponentModel;
#if NETFX_CORE
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Core;
#else
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Provides the plumbing services needed in the view model for the bootstrapper to handle the binding.
    /// </summary>
    public class CoreData : INotifyPropertyChanged
    {
        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification="While it makes sense in SL & WPF, it doesn't work for Metro apps which need this to not be static.")]
        public void Invoke(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

#if (NETFX_CORE)
            ViewControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
#endif
            action();
#if (NETFX_CORE)
                });
#endif
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
