//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------
namespace MetroDemo
{
    using AtomicMVVM;
    using Windows.UI.Xaml.Controls;

    partial class MainPage : Page, IShell
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public void ChangeContent(UserControl viewContent)
        {
            this.Content = viewContent;
        }
    }
}
