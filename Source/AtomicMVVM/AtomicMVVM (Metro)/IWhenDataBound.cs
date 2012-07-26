//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------
namespace AtomicMVVM
{
    //todo: move to base
    /// <summary>
    /// This interface is applied to Views that need to be aware once they are data bound.
    /// </summary>
    public interface IWhenDataBound
    {
        /// <summary>
        /// This method is raised on the view once the view model is set to the data context.
        /// </summary>
        void DataContextBound();
    }
}
