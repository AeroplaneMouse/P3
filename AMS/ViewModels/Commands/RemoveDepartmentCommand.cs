using System;
using AMS.Events;
using AMS.Models;
using AMS.Views.Prompts;
using System.Windows.Input;
using AMS.Database.Repositories;

namespace AMS.ViewModels.Commands
{
    class RemoveDepartmentCommand : ICommand
    {
        private MainViewModel _main;
        private Department _department;

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
            catch (Exception)
            {
                Features.AddNotification(new Notification("An unknown error occurred. Unable to remove department.", Notification.ERROR),
                    3500);
                return;
            }

            // Validating id
            //TODO: Ingen new repositories!
            _department = new DepartmentRepository().GetById(id);
            if (_department != null)
            {
                if (_department.ID != _main.CurrentDepartment.ID)
                {
                    // TODO: Add check for assets and tags conneced to the department.

                    // Prompting user for confirmation
                    Features.DisplayPrompt(new Confirm($"Are you sure you want to delete { _department.Name }?", PromptElapsed));
                }
                else
                    Features.AddNotification(new Notification("You cannot remove your current department. Please change your department and then try again.", Notification.ERROR),
                        3500);
            }
            else
                Features.AddNotification(new Notification("Removing department failed. Department not found!",
                    Notification.ERROR));
        }

        public void PromptElapsed(object sender, PromptEventArgs e)
        {
            if (e.Result)
            {
                // Removing department
                // TODO: Ingen new repositories!
                if (new DepartmentRepository().Delete(_department))
                {
                    _main.OnPropertyChanged(nameof(_main.Departments));
                    Features.AddNotification(new Notification(
                        $"{ _department.Name } has now been removed from the system.", Notification.APPROVE));
                }
                else
                    Features.AddNotification(new Notification(
                        "An unknown error occurred. Unable to remove department.", Notification.ERROR));
            }
        }
    }
}