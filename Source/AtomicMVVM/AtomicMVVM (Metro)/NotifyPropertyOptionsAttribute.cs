//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------
namespace AtomicMVVM
{
    //todo: move to base
    using System;

    public enum MethodDispatchMode
    {
        Async,
        Sync
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class NotifyPropertyOptionsAttribute : Attribute
    {
        public NotifyPropertyOptionsAttribute(MethodDispatchMode methodDispatchMode)
        {
            this.MethodDispatchMode = methodDispatchMode;
        }

        public MethodDispatchMode MethodDispatchMode { get; set; }
    }
}
