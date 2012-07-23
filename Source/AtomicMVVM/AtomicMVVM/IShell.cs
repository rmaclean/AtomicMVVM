//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

namespace AtomicMVVM
{
#if NETFX_CORE
    using Windows.UI.Xaml.Controls;
#else
    using System.Windows.Controls;
#endif

    /// <summary>
    /// The shell which will host the various views.
    /// </summary>
    public interface IShell
    {
        /// <summary>
        /// This is called when the view content changes and needs to be injected into the shell.
        /// </summary>
        /// <param name="viewContent">Content of the view to be loaded into the shell.</param>
        void ChangeContent(UserControl viewContent);
    }
}
