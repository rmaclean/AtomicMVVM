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
