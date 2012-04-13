
namespace WP71Demo
{
    using System.Windows.Controls;
    using AtomicMVVM;
    using Microsoft.Phone.Controls;

    public partial class MainPage : PhoneApplicationPage, IShell
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        public void ChangeContent(UserControl viewContent)
        {
            ContentPanel.Content = viewContent;
        }
    }
}