//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

using AtomicMVVM;

namespace GeneralDemo.ViewModels
{
    public class Menu : CoreData
    {
        public void ShowPopup()
        {
            App.Bootstrapper.ChangeView<Popup>();
        }

        public void ShowCars()
        {
            App.Bootstrapper.ChangeView<Cars>();
        }

        public void ShowCustomers()
        {
            App.Bootstrapper.ChangeView<Customers>();
        }
    }
}
