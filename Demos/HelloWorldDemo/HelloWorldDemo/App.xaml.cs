

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
        public static Bootstrapper Bootstrapper { get; set; }

        public App()
        {
            Bootstrapper = new AtomicMVVM.Bootstrapper();
            Bootstrapper.Start<MainWindow, HelloWorld>();
        }
    }
}
