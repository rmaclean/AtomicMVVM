//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------


namespace AtomicMVVM
{
#if WINRT
    using Windows.UI.Xaml;
#else
    using System.Windows;
#endif

    /// <summary>
    /// Provides a way to attach a command name to an object - this should allow for the same command to be attached to multiple items.
    /// </summary>
    public class UIExtensions
    {
        /// <summary>
        /// Gets the value from the object
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>The value</returns>
        public static string GetCommandName(DependencyObject obj)
        {
            return (string)obj.GetValue(CommandNameProperty);
        }

        /// <summary>
        /// Sets the value on the object.
        /// </summary>
        /// <param name="obj">The object to update.</param>
        /// <param name="value">The value.</param>
        public static void SetCommandName(DependencyObject obj, string value)
        {
            obj.SetValue(CommandNameProperty, value);
        }

        /// <summary>
        /// The command name used when binding commands.S
        /// </summary>
        public static readonly DependencyProperty CommandNameProperty = DependencyProperty.RegisterAttached("CommandName", typeof(string), typeof(UIExtensions), null);
    }
}
