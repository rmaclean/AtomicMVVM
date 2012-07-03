
namespace MetroDemo
{
    using AtomicMVVM;
    using MetroDemo.ViewModels;
    using Windows.ApplicationModel.Activation;
    using Windows.UI.ViewManagement;
    using Windows.UI.WebUI;
    using Windows.UI.Xaml;

    partial class App
    {
        public static Bootstrapper Bootstrapper { get; private set; }

        public App()
        {            
            InitializeComponent();            

            Bootstrapper = new Bootstrapper();
            Bootstrapper.GlobalCommands.Add("BackButton", () =>
            {
                App.Current.Exit();
            });
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                Window.Current.Activate();
                return;
            }

            Bootstrapper.Start<MainPage, Windows8, string>("South Africa");
        }

        protected override void OnSearchActivated(SearchActivatedEventArgs args)
        {
            Bootstrapper.Start<MainPage, Windows8, string>(args.QueryText);
        }

    }
}
