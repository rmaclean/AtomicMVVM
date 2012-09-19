//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------


namespace GeneralDemo
{
    using System.Windows.Controls;
    using AtomicMVVM;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow , IShell
    {
        public MainWindow()
        {
            InitializeComponent();            
        }

        public void ChangeContent(UserControl viewContent)
        {
            this.ContentPlacement.Content = viewContent;
        }
    }
}
