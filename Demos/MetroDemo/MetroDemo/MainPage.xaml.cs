using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtomicMVVM;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace MetroDemo
{
    partial class MainPage : IShell
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public void ChangeContent(UserControl viewContent)
        {
            this.Content = viewContent;
        }
    }
}
