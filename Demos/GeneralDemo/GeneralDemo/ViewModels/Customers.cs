//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------


using System.Linq;
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
                RaisePropertyChanged();
            }
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
