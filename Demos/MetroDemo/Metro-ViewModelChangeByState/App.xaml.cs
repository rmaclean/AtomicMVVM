//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

namespace Metro_ViewModelChangeByState
{
    using System;
    using AtomicMVVM;
    using Metro_ViewModelChangeByState.ViewModels;
    using Windows.ApplicationModel;
    using Windows.ApplicationModel.Activation;
    using Windows.UI.Xaml;

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static Bootstrapper BootStrapper { get; private set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Do not repeat app initialization when already running, just ensure that
            // the window is active
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                Window.Current.Activate();
                return;
            }

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //TODO: Load state from previously suspended application
            }

            Window.Current.SizeChanged += Current_SizeChanged;

            BootStrapper = new Bootstrapper();
            BootStrapper.Start(typeof(MainPage), StateToType());
        }

        private Type StateToType()
        {
            switch (Windows.UI.ViewManagement.ApplicationView.Value)
            {
                case Windows.UI.ViewManagement.ApplicationViewState.Filled:
                    {
                        return typeof(C);
                    }
                case Windows.UI.ViewManagement.ApplicationViewState.FullScreenLandscape:
                    {
                        return typeof(A);
                    }
                case Windows.UI.ViewManagement.ApplicationViewState.FullScreenPortrait:
                    {
                        return typeof(D);
                    }
                case Windows.UI.ViewManagement.ApplicationViewState.Snapped:
                    {
                        return typeof(B);
                    }
            }

            throw new Exception();
        }

        void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            BootStrapper.ChangeView(StateToType());        
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
