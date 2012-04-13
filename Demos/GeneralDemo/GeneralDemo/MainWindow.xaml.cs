

namespace GeneralDemo
{
    using System.Linq;
    using System.Windows.Controls;
    using AtomicMVVM;
    using MahApps.Metro;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow , IShell
    {
        public MainWindow()
        {
            InitializeComponent();
            ThemeManager.ChangeTheme(this, ThemeManager.DefaultAccents.Single(_ => _.Name == "Blue"), Theme.Light);
            this.BindGlobalCommands(App.Bootstrapper);
        }

        public void ChangeContent(UserControl viewContent)
        {
            this.ContentPlacement.Content = viewContent;
        }
    }
}
