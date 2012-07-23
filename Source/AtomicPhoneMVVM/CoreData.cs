//-----------------------------------------------------------------------
// Project: AtomicPhoneMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------  

namespace AtomicPhoneMVVM
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using Microsoft.Phone.Controls;

    public class CoreData : INotifyPropertyChanged
    {
        public IPushMessage Page { get; set; }

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Navigate(string page)
        {
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(page, UriKind.Relative));
        }

        public void PushMessage(string message)
        {
            if (Page != null)
            {
                Page.Push(message);
            }
        }
    }
}
