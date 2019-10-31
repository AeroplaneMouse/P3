using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Events;
using Asset_Management_System.Models;
using System;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels.Commands
{
    class RemoveDepartmentCommand : ICommand
    {
        private MainViewModel _main;
        private Department departmentToRemove;
        private bool promtResult;

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
            // Retrieving department ID
            ulong id;
            try
            {
                id = ulong.Parse(parameter.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR! Removing department failed... { Environment.NewLine } {e.Message}");
                _main.AddNotification(new Notification("ERROR! An unknown error occurred. Unable to remove department.", Notification.ERROR));
                return;
            }

            // Validating id
            DepartmentRepository rep = new DepartmentRepository();
            Department department = rep.GetById(id);
            if (department != null)
            {
                if (department.ID != _main.CurrentDepartment.ID)
                {
                    // TODO: Add check for assets and tags conneced to the department.

                    // Promting user for confirmation
                    departmentToRemove = department;
                    _main.DisplayPromt(new Views.Promts.Confirm($"Are you sure you want to delete { department.Name }?", PromtElapsed));
                }
                else
                    _main.AddNotification(new Notification("ERROR! You cannot remove your current department. Please change your department and then try again.", Notification.ERROR), 3500);            
            }
            else
                _main.AddNotification(new Notification("ERROR! Removing department failed. Department not found!", Notification.ERROR));
        }

        public void PromtElapsed(object sender, PromtEventArgs e)
        {
            if (e.Result)
            {
                // Removing department
                if (new DepartmentRepository().Delete(departmentToRemove))
                {
                    _main.OnPropertyChanged(nameof(_main.Departments));
                    _main.AddNotification(new Notification($" {departmentToRemove.Name } has now been removed from the system.", Notification.APPROVE));
                }
                else
                    _main.AddNotification(new Notification("ERROR! An unknown error occurred. Unable to remove department.", Notification.ERROR));
            }
        }
    }
}
