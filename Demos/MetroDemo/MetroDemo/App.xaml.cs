//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------
namespace MetroDemo
{
    using AtomicMVVM;
    using MetroDemo.ViewModels;
    using MetroDemo.ViewModels.Contracts;
    using Windows.ApplicationModel.Activation;
    using Windows.ApplicationModel.DataTransfer;
    using Windows.ApplicationModel.Search;
    using Windows.UI.Xaml;

    partial class App
    {
        public static Bootstrapper Bootstrapper { get; private set; }

        public App()
        {            
            InitializeComponent();            

            Bootstrapper = new Bootstrapper();
            Bootstrapper.AfterStartCompletes += Bootstrapper_AfterStartCompletes;
            Bootstrapper.GlobalCommands.Add("BackButton", () =>
            {
                App.Current.Exit();
            });
        }

        void Bootstrapper_AfterStartCompletes()
        {
            var searchPane = SearchPane.GetForCurrentView();
            searchPane.QuerySubmitted += (sender, args) =>
            {
                var search = Bootstrapper.CurrentViewModel as ISearch;
                if (search != null)
                {
                    search.Search(args.QueryText);
                }
              
            };

            var dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += (sender, args) =>
            {
                var share = Bootstrapper.CurrentViewModel as IShare;
                if (share != null)
                {
                    share.Share(args.Request);
                }                
            };
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
