//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

namespace MetroDemo.Views.FullScreenPortrait
{
    using AtomicMVVM;

    public sealed partial class Windows8: IWhenDataBound
    {
        private ViewModels.Windows8 vm;
        public Windows8()
        {
            InitializeComponent();            
        }

        private void SearchBoxKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {                
                vm.Search = query.Text;
                vm.Refresh();
            }
        }

        public void DataContextBound<T>(T viewModel) where T : CoreData
        {
            this.vm = this.DataContext as ViewModels.Windows8;
        }
    }
}
