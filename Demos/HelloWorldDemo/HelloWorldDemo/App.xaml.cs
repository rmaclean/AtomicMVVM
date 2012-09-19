//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------


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
