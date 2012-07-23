//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

namespace AtomicMVVM
{
    using System;
    using System.Collections.Generic;

#if WINDOWS_PHONE
    using ActionCommand = Tuple<string, System.Action>;
#else
    using ActionCommand = System.Tuple<string,System.Action>;
#endif

    /// <summary>
    /// Extension methods for AtomicMVVM
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Adds the specified command to the global commands.
        /// </summary>
        /// <param name="globalCommands">The global commands.</param>
        /// <param name="commandId">The command id.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="System.ArgumentNullException">If any of the parameters are null.</exception>
        public static void Add(this List<ActionCommand> globalCommands, string commandId, Action action)
        {
            if (string.IsNullOrWhiteSpace(commandId))
            {
                throw new ArgumentNullException("commandId");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (globalCommands == null)
            {
                throw new ArgumentNullException("globalCommands");
            }

            var actionCommand = new ActionCommand(commandId, action);
            globalCommands.Add(actionCommand);  
        }
    }
}
