using System.Windows;
using AtomicMVVM;
using HelloWorldDemo.ViewModels;

namespace HelloWorldDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Bootstrapper<MainWindow, HelloWorld> Bootstrapper = new Bootstrapper<MainWindow,HelloWorld>();
    }
}
