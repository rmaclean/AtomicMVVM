//-----------------------------------------------------------------------
// Project: AtomicPhoneMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

namespace AtomicPhoneMVVM
{
    using System.Windows;
    public class XAMLExtensions
    {
        public static readonly DependencyProperty CommandNameProperty = DependencyProperty.RegisterAttached("CommandName", typeof(string), typeof(XAMLExtensions), new PropertyMetadata(string.Empty));

        public static void SetCommandName(UIElement element, string value)
        {
            element.SetValue(CommandNameProperty, value);
        }

        public static string GetCommandName(UIElement element)
        {
            return (string)element.GetValue(CommandNameProperty);
        }
    }  
}
