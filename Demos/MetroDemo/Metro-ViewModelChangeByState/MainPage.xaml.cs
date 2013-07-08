//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

namespace Metro_ViewModelChangeByState
{
    using AtomicMVVM;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IShell
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        public void ChangeContent(UserControl viewContent)
        {
            this.Content = viewContent;
        }
    }
}
