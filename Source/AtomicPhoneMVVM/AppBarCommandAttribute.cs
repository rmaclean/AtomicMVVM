//-----------------------------------------------------------------------
// Project: AtomicPhoneMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

namespace AtomicPhoneMVVM
{
    using System;

    /// <summary>
    /// Used to attribute methods that are associated to an AppBar command based on the title text of the AppBar command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]   
    public sealed class AppBarCommandAttribute : Attribute
    {
        /// <summary>
        /// The AppBar title text to associate the command with.
        /// </summary>
        public string AppBarText { get; private set; }

        /// <summary>
        /// Creates an instance of the AppBarCommandAttribute class.
        /// </summary>
        /// <param name="appBarText">The AppBar title text to associate the command with.</param>
        public AppBarCommandAttribute(string appBarText)
        {
            this.AppBarText = appBarText;
        }
    }
}
