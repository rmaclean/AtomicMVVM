//todo: document this & move to main project

namespace AtomicMVVM
{
    using Windows.UI.Xaml;

    public class UIExtensions
    {
        public static string GetCommandName(DependencyObject obj)
        {
            return (string)obj.GetValue(CommandNameProperty);
        }

        public static void SetCommandName(DependencyObject obj, string value)
        {
            obj.SetValue(CommandNameProperty, value);
        }

        public static readonly DependencyProperty CommandNameProperty = DependencyProperty.RegisterAttached("CommandName", typeof(string), typeof(UIExtensions), null);
    }
}
