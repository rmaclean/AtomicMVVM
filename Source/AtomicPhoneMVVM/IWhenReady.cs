//-----------------------------------------------------------------------
// Project: AtomicPhoneMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

namespace AtomicPhoneMVVM
{
    /// <summary>
    /// Allows the view to raise an method when the databinding is completed.
    /// </summary>
    public interface IWhenReady
    {
        /// <summary>
        /// Is raised by the boot strapper when the databinding is complete.
        /// </summary>
        void Ready();
    }
}
