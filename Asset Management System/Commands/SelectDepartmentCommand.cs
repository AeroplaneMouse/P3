using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Asset_Management_System.Commands
{
    class SelectDepartmentCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private ViewModels.MainViewModel _main;
        public SelectDepartmentCommand(ViewModels.MainViewModel main)
        {
            _main = main;
        }



        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object sender)
        {
            ulong id = ulong.Parse(sender.ToString());
            Models.Department selectedDepartment = new Database.Repositories.DepartmentRepository().GetById(id);
            if (selectedDepartment == null)
                selectedDepartment = Models.Department.GetDefault();

            _main.AddNotification(new Models.Notification($"Selecting new department: { selectedDepartment.Name }", Models.Notification.APPROVE));
            _main.CurrentDepartment = selectedDepartment;
        }
    }
}
