using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
