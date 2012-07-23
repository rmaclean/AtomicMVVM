//-----------------------------------------------------------------------
// Project: AtomicPhoneMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

namespace AtomicPhoneMVVM
{
    using System;
    using System.Reflection;
    using System.Windows.Input;

    class AttachedCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            if (!canExecuteExists)
            {
                return true;
            }

            if (parameter == null)
            {
                return false;
            }

            if (canExecuteMethod == null)
            {
                canExecuteMethod = parameter.GetType().GetMethod("Can" + this.methodName, Type.EmptyTypes);
            }

            return (bool)canExecuteMethod.Invoke(parameter, null);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, new EventArgs());
            }
        }

        public event EventHandler CanExecuteChanged;
        private string methodName;
        private MethodInfo executeMethod;
        private MethodInfo canExecuteMethod;
        private bool canExecuteExists;

        public AttachedCommand(string methodName, bool canExecuteExists)
        {
            this.methodName = methodName;
            this.canExecuteExists = canExecuteExists;
        }

        public void Execute(object parameter)
        {
            if (executeMethod == null)
            {
                executeMethod = parameter.GetType().GetMethod(this.methodName, Type.EmptyTypes);
            }

            executeMethod.Invoke(parameter, null);
        }
    }
}
