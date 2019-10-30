using Asset_Management_System.Database.Repositories;
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
            // Get id
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

            // Validate id
            DepartmentRepository rep = new DepartmentRepository();
            Department department = rep.GetById(id);
            if (department != null)
                // Promt the user for confirmation
                // TODO: Add method in main to handle popup promts
                _main.PopupPage.Content = new Views.Promts.Confirm(RemovalApproved, out promtResult);
            else
                _main.AddNotification(new Notification("ERROR! Removing department failed. Department not found!", Notification.ERROR));
        }

        public void RemovalApproved()
        {
            if (new DepartmentRepository().Delete(departmentToRemove))
                _main.AddNotification(new Notification("Department", Notification.APPROVE));
            else
                _main.AddNotification(new Notification("ERROR! An unknown error occurred. Unable to remove department.", Notification.ERROR));
        }
    }
}
