using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels.Commands
{
    class RemoveNotificationCommand : ICommand
    {
        private MainViewModel _main;
        public event EventHandler CanExecuteChanged;

        public RemoveNotificationCommand(MainViewModel main)
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
            _main.RemoveNotification(id);
        }
    }
}