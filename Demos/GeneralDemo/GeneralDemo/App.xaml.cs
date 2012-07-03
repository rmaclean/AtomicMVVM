
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
        public static Bootstrapper Bootstrapper { get; set; }
        public static CustomerData CustomerData {get;set;}

        public App()
        {
            CustomerData = new CustomerData();
            Bootstrapper = new Bootstrapper();
            Bootstrapper.GlobalCommands.Add("GoToMenu", () => App.Bootstrapper.ChangeView<Menu>());
            Bootstrapper.GlobalCommands.Add("AboutAtomicMVVM", () =>
            {
                Process.Start("https://bitbucket.org/rmaclean/atomicmvvm");
            });

            Bootstrapper.Start<MainWindow, Menu>();
        }
    }
}
