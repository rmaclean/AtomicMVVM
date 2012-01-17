using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AtomicMVVM;

namespace GeneralDemo.ViewModels
{
    public class Popup : CoreData
    {
        private string _fullName;

        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;
                RaisePropertyChanged("FullName");
            }
        }

        public void ShowName()
        {
            MessageBox.Show("Hello "+this.FullName);
            App.Bootstrapper.ChangeView<Menu>();
        }

    }
}
