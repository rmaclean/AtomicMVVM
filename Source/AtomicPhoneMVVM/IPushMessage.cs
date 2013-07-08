//-----------------------------------------------------------------------
// Project: AtomicPhoneMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

namespace AtomicPhoneMVVM
{
    /// <summary>
    /// Enables a view to recieve messages.
    /// </summary>
    public interface IPushMessage
    {
        /// <summary>
        /// Is raised when a view model pushes a message to the view.
        /// </summary>
        /// <param name="message">The message.</param>
        void Push(string message);
    }
}
