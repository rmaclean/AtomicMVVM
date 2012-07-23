//-----------------------------------------------------------------------
// Project: AtomicPhoneMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

namespace AtomicPhoneMVVM
{
    using System;

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
