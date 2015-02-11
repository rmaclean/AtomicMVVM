//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

namespace AtomicMVVM
{
    using System;
    using System.Reflection;
    using System.Windows.Input;

    internal class AttachedCommand : ICommand
    {
#if (WINRT)
        private readonly Type[] EmptyTypes = new Type[] { };
#else
        private readonly Type[] EmptyTypes = Type.EmptyTypes;
#endif

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
#if WINRT
                canExecuteMethod = parameter.GetType().GetRuntimeMethod("Can" + this.methodName, EmptyTypes);
#else
                canExecuteMethod = parameter.GetType().GetMethod("Can" + this.methodName, EmptyTypes);
#endif
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

        private readonly string methodName;
        private MethodInfo executeMethod;
        private MethodInfo canExecuteMethod;
        private readonly bool canExecuteExists;

        public AttachedCommand(string methodName, bool canExecuteExists)
        {
            this.methodName = methodName;
            this.canExecuteExists = canExecuteExists;
        }

        public void Execute(object parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            if (executeMethod == null)
            {
#if WINRT
                executeMethod = parameter.GetType().GetRuntimeMethod(this.methodName, EmptyTypes);
#else
                executeMethod = parameter.GetType().GetMethod(this.methodName, EmptyTypes);
#endif
            }

            if (executeMethod == null)
            {
                throw new Exception("Unable to find a public method named " + methodName);
            }

            executeMethod.Invoke(parameter, null);
        }
    }
}