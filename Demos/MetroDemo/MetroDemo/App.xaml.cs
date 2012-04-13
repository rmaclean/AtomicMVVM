
namespace MetroDemo
{
    using System;
    using System.Collections.Generic;
    using AtomicMVVM;
    using MetroDemo.ViewModels;
    using Windows.ApplicationModel.Activation;

    partial class App
    {
        public static Bootstrapper<MainPage, Windows8> Bootstrapper;

        public App()
        {
            InitializeComponent();            
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {       
            Bootstrapper = new Bootstrapper<MainPage, Windows8>();
            Bootstrapper.GlobalCommands.Add("BackButton", () =>
            {
                App.Current.Exit();
            });

            Bootstrapper.Start();
        }
    }
}
