using System;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels.Commands
{
    class RemoveDepartmentCommand : ICommand
    {
        private MainViewModel _main;
        public event EventHandler CanExecuteChanged;

        public RemoveDepartmentCommand(MainViewModel main)
        {
            _main = main;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            int id = int.Parse(parameter.ToString());
            _main.AddNotification(new Models.Notification($"Removing department: { id }", Models.Notification.WARNING));
            
        }
    }
}
