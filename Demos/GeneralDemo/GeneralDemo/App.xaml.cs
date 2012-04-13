
namespace GeneralDemo
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using AtomicMVVM;
    using GeneralDemo.Models;
    using GeneralDemo.ViewModels;

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
            Bootstrapper.GlobalCommands.Add("GoToMenu", () => App.Bootstrapper.ChangeView<Menu>());
            Bootstrapper.GlobalCommands.Add("AboutAtomicMVVM", () =>
            {
                Process.Start("https://bitbucket.org/rmaclean/atomicmvvm");
            });
            Bootstrapper.Start();
        }
    }
}
