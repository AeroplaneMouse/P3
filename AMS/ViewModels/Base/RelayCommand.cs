using System;
using System.Windows.Input;

namespace AMS.ViewModels.Base
{
    // Command heavily inspired by the RelayCommand found in 
    internal class RelayCommand : ICommand
    {
        // The action to run
        private readonly Action _action;

        // The function that checks if the action should run
        private readonly Func<bool> _func;

        public event EventHandler CanExecuteChanged = (sender, e) => { };

        public RelayCommand(Action action, Func<bool> func = null)
        {
            _action = action;
            _func = func;
        }

      
        public bool CanExecute(object parameter)
        {
            if (_func != null)
                return _func();

            return true;
        }

        /// <summary>
        /// Executes the Action
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            _action();
        }
    }

    // Command heavily inspired by the RelayCommand found in 
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _action;

        public RelayCommand(Action<T> action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }
        
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        
        public void Execute(object parameter)
        {
            _action((T) parameter);
        }
    }
}