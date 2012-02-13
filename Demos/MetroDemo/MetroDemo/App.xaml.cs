
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
            var commands = new List<Tuple<string,Action>>();
            commands.Add(new Tuple<string, Action>("BackButton", () =>
            {
                App.Current.Exit();
            }));

            Bootstrapper = new Bootstrapper<MainPage, Windows8>(commands);
        }
    }
}