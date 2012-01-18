using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AtomicMVVM;
using GeneralDemo.Models;
using GeneralDemo.ViewModels;

namespace GeneralDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Bootstrapper<MainWindow, Menu> Bootstrapper;
        public static CustomerData CustomerData = new CustomerData();

        public App()
        {
            Bootstrapper = new Bootstrapper<MainWindow, Menu>();
        }
    }
}
