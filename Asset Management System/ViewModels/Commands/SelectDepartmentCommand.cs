using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Services.Interfaces;

namespace Asset_Management_System.ViewModels.Commands
{
    class SelectDepartmentCommand : ICommand
    {
        private ViewModels.MainViewModel _main;
        private IDepartmentService _service;
        private IDepartmentRepository _rep;
        public event EventHandler CanExecuteChanged;

        public SelectDepartmentCommand(MainViewModel main, IDepartmentService service)
        {
            _main = main;
            _service = service;
            _rep = service.GetRepository() as IDepartmentRepository;
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
                Department selectedDepartment = _rep.GetById(id);
                if (selectedDepartment == null)
                    selectedDepartment = Models.Department.GetDefault();

                _main.AddNotification(new Models.Notification(
                    $"{selectedDepartment.Name} is now the current department.", Models.Notification.APPROVE));
                _main.CurrentDepartment = selectedDepartment;
            }
            catch (Exception e)
            {
                _main.AddNotification(new Models.Notification(e.Message, Models.Notification.ERROR), 5000);
            }
        }
    }
}