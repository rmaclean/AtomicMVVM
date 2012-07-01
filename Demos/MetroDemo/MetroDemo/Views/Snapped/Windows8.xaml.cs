namespace MetroDemo.Views.Snapped
{
    using Windows.UI.Xaml.Controls;


    public sealed partial class Windows8 : UserControl
    {
        public Windows8()
        {
            this.InitializeComponent();
        }

        private void SearchBoxKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var vm = this.DataContext as ViewModels.Windows8;
                vm.Search = query.Text;
                vm.Refresh();
            }
        }
    }
}
