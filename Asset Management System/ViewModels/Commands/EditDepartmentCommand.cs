using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels.Commands
{
    class EditDepartmentCommand : ICommand
    {
        private MainViewModel _main;
        public event EventHandler CanExecuteChanged;

        public EditDepartmentCommand(MainViewModel main)
        {
            _main = main;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            try
            {
                int id = int.Parse(parameter.ToString());

                _main.AddNotification(new Models.Notification($"Editing department { id }.", Models.Notification.WARNING));

            }
            catch (Exception)
            {
                _main.AddNotification(new Models.Notification("ERROR! Unable to edit department...", Models.Notification.ERROR));
            }
        }
    }
}
