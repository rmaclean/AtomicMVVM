//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

namespace AtomicMVVM
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Defines a command that can be attached to a button, if no match is found in the model.
    /// </summary>
    public sealed class GlobalCommand : ICommand
    {
        /// <summary>
        /// Determines whether this instance can execute the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        ///   <c>true</c> if this instance can execute the specified parameter; otherwise, <c>false</c>.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

#pragma warning disable 67
        /// <summary>
        /// Occurs when [can execute changed].
        /// </summary>
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67

        private readonly Action action;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalCommand" /> class.
        /// </summary>
        /// <param name="action">The action to run.</param>
        public GlobalCommand(Action action)
        {
            this.action = action;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <remarks>The parameter is ignored.</remarks>
        public void Execute(object parameter)
        {
            action();
        }
    }   
}
