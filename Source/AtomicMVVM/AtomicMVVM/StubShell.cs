//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

namespace AtomicMVVM
{
#if WINRT

    using Windows.UI.Xaml.Controls;

#else

    using System.Windows.Controls;

#endif

    /// <summary>
    /// A dummy (headless) shell.
    /// </summary>
    /// <remarks>
    /// This is mostly used to fake loading for unit testing scenarios.
    /// </remarks>
    public class StubShell : IShell
    {
        /// <summary>
        /// This is called when the view content changes and needs to be injected into the shell.
        /// </summary>
        /// <param name="viewContent">Content of the view to be loaded into the shell.</param>
        /// <remarks>Does nothing in StubShell</remarks>
        public void ChangeContent(UserControl viewContent)
        {
            // sure thing :)
        }
    }
}