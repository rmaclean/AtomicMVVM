
namespace Silverlight5
{
    using System.Windows.Controls;
    using AtomicMVVM;

    public partial class MainPage : UserControl, IShell
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
