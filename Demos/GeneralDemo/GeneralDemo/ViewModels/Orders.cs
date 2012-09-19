//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

using System;
using System.Linq;
using AtomicMVVM;

namespace GeneralDemo.ViewModels
{
    using Order = Tuple<string, DateTime, double>;

    public class Orders : CoreData
    {
        public Order[] OrderList { get; private set; }

        public Orders(string customerName)
        {
            OrderList = App.CustomerData.Data.Single(_ => _.Name == customerName).Data;
        }

        public void GoToCustomers()
        {
            App.Bootstrapper.ChangeView<Customers>();
        }
    }
}
