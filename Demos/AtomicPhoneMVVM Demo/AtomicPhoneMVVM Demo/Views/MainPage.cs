using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using AtomicPhoneMVVM;
using AtomicStorage;

namespace AtomicPhoneMVVMDemo.Views
{
    public class MainPage : CoreData, IDataStore
    {
        private string _name;
        private string message;

        public string Message
        {
            get { return message; }
            set
            {
                if (value != message)
                {
                    message = value;
                    RaisePropertyChanged("Message");
                }
            }
        }

        [Storage("Name")]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged("Name");
                    this.SaveSettings();
                }
            }
        }

        public MainPage()
        {
            this.LoadSettings();
        }

        public void SayHi()
        {
            Message = "Hi " + Name;
        }

        [ReevaluateProperty("Name")]
        public bool CanSayHi()
        {
            return !string.IsNullOrWhiteSpace("Name");
        }

    }
}
