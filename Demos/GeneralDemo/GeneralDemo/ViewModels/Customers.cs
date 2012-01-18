using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtomicMVVM;

namespace GeneralDemo.ViewModels
{
    public class Customers : CoreData
    {
        public string[] CustomerNames { get; set; }

        private string _selectedCustomer;

        public string SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;
                RaisePropertyChanged("SelectedCustomer");
            }
        }

        public void GoToMenu()
        {
            App.Bootstrapper.ChangeView<Menu>();
        }

        public void ViewOrders()
        {
            App.Bootstrapper.ChangeView<Orders,string>(SelectedCustomer);
        }

        [ReevaluateProperty("SelectedCustomer")]
        public bool CanViewOrders()
        {
            return !string.IsNullOrWhiteSpace(SelectedCustomer);
        }

        public Customers()
        {
            CustomerNames = App.CustomerData.Data.Select(_ => _.Name).ToArray();
        }
    }
}
