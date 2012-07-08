

namespace AtomicMVVM
{
    using System;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void BindGlobalCommands(this ContentControl shell, Bootstrapper bootStrapper)
        {
            if (bootStrapper == null)
            {
                throw new ArgumentNullException("bootStrapper");
            }

            bootStrapper.BindGlobalCommands(shell, bootStrapper.GlobalCommands);
        }
    }
}
