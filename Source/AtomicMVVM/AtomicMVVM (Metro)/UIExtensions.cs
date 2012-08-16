//todo: document this & move to main project

namespace AtomicMVVM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
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

        // Using a DependencyProperty as the backing store for CommandName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandNameProperty = DependencyProperty.RegisterAttached("CommandName", typeof(string), typeof(UIExtensions), null);
    }
}
