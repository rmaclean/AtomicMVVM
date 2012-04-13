

namespace HelloWorldDemo
{
    using System.Windows;
    using AtomicMVVM;
    using HelloWorldDemo.ViewModels;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Bootstrapper<MainWindow, HelloWorld> Bootstrapper = new Bootstrapper<MainWindow,HelloWorld>();

        public App()
        {
            Bootstrapper.Start();
        }
    }
}
