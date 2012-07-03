

namespace AtomicMVVM
{
#if NETFX_CORE
    using Windows.UI.Xaml.Controls;
#else
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Extensions for the IShell class
    /// </summary>
    /// <seealso cref="IShell"/>
    public static class IShellExtensions
    {
        /// <summary>
        /// Binds the global commands to buttons in the shell.
        /// </summary>
        /// <param name="shell">The shell.</param>
        /// <param name="bootStrapper">The bootstrapper.</param>
        public static void BindGlobalCommands(this ContentControl shell, Bootstrapper bootStrapper)
        {
            bootStrapper.BindGlobalCommands(shell, bootStrapper.GlobalCommands);
        }
    }
}
