
namespace GeneralDemo
{
    using System.Diagnostics;
    using System.Windows;
    using AtomicMVVM;
    using GeneralDemo.Models;
    using GeneralDemo.ViewModels;
    using MahApps.Metro;
    using System.Linq;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Bootstrapper Bootstrapper { get; private set; }
        public static CustomerData CustomerData { get; private set; }

        public App()
        {
            CustomerData = new CustomerData();
            Bootstrapper = new Bootstrapper();
            Bootstrapper.GlobalCommands.Add("GoToMenu", () => App.Bootstrapper.ChangeView<ViewModels.Menu>());
            Bootstrapper.GlobalCommands.Add("AboutAtomicMVVM", () =>
            {
                Process.Start("https://bitbucket.org/rmaclean/atomicmvvm");
            });

            Bootstrapper.AfterStartCompletes += Bootstrapper_AfterStartCompletes;

            Bootstrapper.Start<MainWindow, ViewModels.Menu>();
        }

        void Bootstrapper_AfterStartCompletes()
        {
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.Single(_ => _.Name == "Blue"), Theme.Light);
            (Bootstrapper.CurrentShell as ContentControl).BindGlobalCommands(App.Bootstrapper);
        }
    }
}
