using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
