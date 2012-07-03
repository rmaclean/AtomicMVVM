

namespace AtomicMVVM
{
    using System;
    using System.Collections.Generic;

#if WINDOWS_PHONE
    using ActionCommand = Tuple<string, System.Action>;
#else
    using ActionCommand = System.Tuple<string,System.Action>;
#endif

    public static class Extensions
    {
        /// <summary>
        /// Adds the specified command to the global commands.
        /// </summary>
        /// <param name="commandId">The command id.</param>
        /// <param name="action">The action.</param>
        public static void Add(this List<ActionCommand> globalCommands, string commandId, Action action)
        {
            var actionCommand = new ActionCommand(commandId, action);
            globalCommands.Add(actionCommand);  
        }
    }
}
