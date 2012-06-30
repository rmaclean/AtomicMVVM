
namespace MetroDemo
{
    using System;
    using System.Collections.Generic;
    using AtomicMVVM;
    using MetroDemo.ViewModels;
    using Windows.ApplicationModel.Activation;

    partial class App
    {
        public static Bootstrapper Bootstrapper { get; set; }

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
            Bootstrapper.Start<MainPage, Windows8, string>("South Africa");
        }

        protected override void OnSearchActivated(SearchActivatedEventArgs args)
        {
            Bootstrapper.Start<MainPage, Windows8, string>(args.QueryText); 
        }
    }
}
