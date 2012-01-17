using System.Windows;
using System.Windows.Controls;
using AtomicMVVM;

namespace HelloWorldDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IShell
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void ChangeContent(UserControl viewContent)
        {
            ContentPlaceHolder.Content = viewContent;
        }
    }
}
