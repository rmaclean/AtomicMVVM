﻿//-----------------------------------------------------------------------
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

#if WINRT
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Core;
#else

    using System.Windows.Controls;

#endif

    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Provides the plumbing services needed in the view model for the bootstrapper to handle the binding.
    /// </summary>
    public class CoreData : INotifyPropertyChanged
    {
        private Dictionary<string, MethodDispatchMode> propertyDispatchModes;

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null && ViewControl != null)
            {
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
#if WINRT
                if (methodDispatchMode == MethodDispatchMode.Async)
                {
                    var ƒ = ViewControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
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
            }
        }

        /// <summary>
        /// Invokes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="System.ArgumentNullException">If the action provide is null.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "While it makes sense in SL & WPF, it doesn't work for Metro apps which need this to not be static.")]
#pragma warning disable 1998
        public async Task InvokeAsync(Action action)
#pragma warning restore 1998
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (ViewControl == null)
            {
                action();
                return;
            }

#if (WINRT)
            await ViewControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
#endif
#if (NET45)
            await ViewControl.Dispatcher.InvokeAsync(() =>
                {
#endif
                    action();
#if (WINRT || NET45)
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