using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels.Commands
{
    class SelectDepartmentCommand : ICommand
    {
        private ViewModels.MainViewModel _main;
        public event EventHandler CanExecuteChanged;

        public SelectDepartmentCommand(ViewModels.MainViewModel main)
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
                ulong id = ulong.Parse(parameter.ToString());
                Models.Department selectedDepartment = new Database.Repositories.DepartmentRepository().GetById(id);
                if (selectedDepartment == null)
                    selectedDepartment = Models.Department.GetDefault();

                _main.AddNotification(new Models.Notification($"{ selectedDepartment.Name } is now the current department.", Models.Notification.APPROVE));
                _main.CurrentDepartment = selectedDepartment;
            }
            catch(Exception e)
            {
                _main.AddNotification(new Models.Notification(e.Message, Models.Notification.ERROR), 5000);
            }
        }
    }
}
