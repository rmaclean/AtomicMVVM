
namespace AtomicMVVM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class Bootstrapper<TShell, TContent>
        where TShell : IShell
        where TContent : CoreData
    {
        private IShell shell;
        private CoreData viewModel;
        private UserControl view;

        public Bootstrapper()
        {
            shell = (IShell)typeof(TShell).GetConstructor(Type.EmptyTypes).Invoke(null);

            this.ChangeView<TContent>();

            (shell as Window).Show();
        }        

        public void ChangeView<TNewContent>()
            where TNewContent : CoreData
        {
            viewModel = typeof(TNewContent).GetConstructor(Type.EmptyTypes).Invoke(null) as CoreData;
            var viewName = typeof(TNewContent).AssemblyQualifiedName.Replace(".ViewModels.", ".Views.");

            view = Type.GetType(viewName, true, true).GetConstructor(Type.EmptyTypes).Invoke(null) as UserControl;

            var validMethods = (from m in viewModel.GetType().GetMethods()
                                where m.IsPublic &&
                                     !m.IsSpecialName &&
                                     !m.Attributes.HasFlag(MethodAttributes.VtableLayoutMask) &&
                                     m.ReturnType == typeof(void)
                                select m).ToList();

            foreach (var method in validMethods)
            {
                foreach (var attribute in method.GetCustomAttributes<TriggerPropertyAttribute>(false))
                {
                    AddTrigger(attribute.PropertyName, method.Name);
                }

                var control = view.FindName(method.Name);
                if (control != null && control is ICommandSource)
                {
                    var commandProperty = control.GetType().GetProperty("Command");
                    if (commandProperty.GetValue(control) == null)
                    {
                        var commandParameterProperty = control.GetType().GetProperty("CommandParameter");
                        if (commandProperty.CanWrite && commandParameterProperty.CanWrite)
                        {
                            var canExecuteExists = false;
                            var canExecuteMethod = viewModel.GetType().GetMethod("Can" + method.Name, Type.EmptyTypes);
                            if (canExecuteMethod != null)
                            {
                                canExecuteExists = canExecuteMethod.ReturnType == typeof(bool);
                            }

                            var command = new AttachedCommand(method.Name, canExecuteExists);

                            if (canExecuteMethod != null)
                            {
                                foreach (var attribute in canExecuteMethod.GetCustomAttributes<ReevaluatePropertyAttribute>(false))
                                {
                                    viewModel.PropertyChanged += (s, e) =>
                                        {
                                            if (e.PropertyName == attribute.PropertyName)
                                            {
                                                command.RaiseCanExecuteChanged();
                                            }
                                        };
                                }
                            }

                            commandProperty.SetValue(control, command);
                            commandParameterProperty.SetValue(control, viewModel);
                        }
                    }
                }
            }

            view.DataContext = viewModel;

            shell.ChangeContent(view);
        }

        private void AddTrigger(string propertyName, string methodName)
        {
            this.viewModel.PropertyChanged += (s,e) =>
                {
                    if (e.PropertyName == propertyName)
                    {
                        viewModel.GetType().GetMethod(methodName).Invoke(viewModel, null);
                    }
                };
        }
    }

    public class AttachedCommand : ICommand
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
                executeMethod = parameter.GetType().GetMethod(this.methodName);
            }

            executeMethod.Invoke(parameter, null);
        }
    }

    public class CoreData : INotifyPropertyChanged
    {
        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class ReevaluatePropertyAttribute : System.Attribute
    {
        public ReevaluatePropertyAttribute(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        public string PropertyName { get; private set; }
    }

    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class TriggerPropertyAttribute : System.Attribute
    {
        public TriggerPropertyAttribute(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        public string PropertyName { get; private set; }
    }

    public interface IShell
    {
        void ChangeContent(UserControl viewContent);
    }
}
