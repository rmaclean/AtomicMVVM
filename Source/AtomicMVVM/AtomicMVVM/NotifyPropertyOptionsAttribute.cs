//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------
namespace AtomicMVVM
{
    using System;

    /// <summary>
    /// Defines the mode that the INotify is raised.
    /// </summary>
    public enum MethodDispatchMode
    {
        /// <summary>
        /// Event is raised async.
        /// </summary>
        Async,
        /// <summary>
        /// Event is raised sync
        /// </summary>
        Sync
    }

    /// <summary>
    /// Used to attribute properties which implement INotifyPropertyChange so that you can control the way that is invoked.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]    
    public sealed class NotifyPropertyOptionsAttribute : Attribute
    {
        /// <summary>
        /// Creates an instance of NotifyPropertyOptionsAttribute
        /// </summary>
        /// <param name="methodDispatchMode">The mode to use when INotifyPropertyChanged is raised.</param>
        public NotifyPropertyOptionsAttribute(MethodDispatchMode methodDispatchMode)
        {
            this.MethodDispatchMode = methodDispatchMode;
        }

        /// <summary>
        /// The mode to use when INotifyPropertyChanged is raised.
        /// </summary>
        public MethodDispatchMode MethodDispatchMode { get; private set; }
    }
}
