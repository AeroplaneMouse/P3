using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Events;
using Asset_Management_System.Models;
using System;
using System.Windows.Input;
using Asset_Management_System.Services.Interfaces;
using Newtonsoft.Json.Serialization;

namespace Asset_Management_System.ViewModels.Commands
{
    class RemoveDepartmentCommand : ICommand
    {
        private MainViewModel _main;
        private Department departmentToRemove;
        private bool promptResult;
        private IDepartmentService _service;
        private IDepartmentRepository _rep;

        public event EventHandler CanExecuteChanged;

        public RemoveDepartmentCommand(MainViewModel main, IDepartmentService service)
        {
            _main = main;
            _service = service;
            _rep = _service.GetRepository() as IDepartmentRepository;
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
                Console.WriteLine($"ERROR! Removing department failed... {Environment.NewLine} {e.Message}");
                _main.AddNotification(new Notification("ERROR! An unknown error occurred. Unable to remove department.",
                    Notification.ERROR));
                return;
            }

            // Validating id
            Department department = _rep.GetById(id);
            if (department != null)
            {
                if (department.ID != _main.CurrentDepartment.ID)
                {
                    // TODO: Add check for assets and tags conneced to the department.

                    // Prompting user for confirmation
                    departmentToRemove = department;
                    _main.DisplayPrompt(new Views.Prompts.Confirm($"Are you sure you want to delete {department.Name}?",
                        PromptElapsed));
                }
                else
                    _main.AddNotification(
                        new Notification(
                            "ERROR! You cannot remove your current department. Please change your department and then try again.",
                            Notification.ERROR), 3500);
            }
            else
                _main.AddNotification(new Notification("ERROR! Removing department failed. Department not found!",
                    Notification.ERROR));
        }

        public void PromptElapsed(object sender, PromptEventArgs e)
        {
            if (e.Result)
            {
                // Removing department
                if (_rep.Delete(departmentToRemove))
                {
                    _main.OnPropertyChanged(nameof(_main.Departments));
                    _main.AddNotification(new Notification(
                        $" {departmentToRemove.Name} has now been removed from the system.", Notification.APPROVE));
                }
                else
                    _main.AddNotification(new Notification(
                        "ERROR! An unknown error occurred. Unable to remove department.", Notification.ERROR));
            }
        }
    }
}