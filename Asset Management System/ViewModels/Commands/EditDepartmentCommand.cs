using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Events;
using Asset_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels.Commands
{
    class EditDepartmentCommand : ICommand
    {
        private MainViewModel _main;
        private Department editingDepartment;

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
            // Retrieving department ID
            ulong id;
            try
            {
                id = ulong.Parse(parameter.ToString());
            }
            catch (Exception)
            {
                _main.AddNotification(new Models.Notification(
                    $"ERROR! Unknown error. Unable to edit department with id: {parameter.ToString()}",
                    Models.Notification.ERROR));
                return;
            }

            // Validating id
            DepartmentRepository rep = new DepartmentRepository();
            Department department = rep.GetById(id);
            if (department != null)
            {
                editingDepartment = department;
                _main.DisplayPrompt(new Views.Prompts.TextInput("Enter new name", department.Name, PromptElapsed));
            }
            else
                _main.AddNotification(
                    new Notification("ERROR! Editing department failed. Department not found!", Notification.ERROR),
                    3500);
        }

        private void PromptElapsed(object sender, PromptEventArgs e)
        {
            if (e.Result && e.ResultMessage != null)
            {
                if (e.ResultMessage != String.Empty)
                {
                    editingDepartment.Name = e.ResultMessage;
                    if (new DepartmentRepository().Update(editingDepartment))
                    {
                        _main.CurrentDepartment = editingDepartment;
                        _main.OnPropertyChanged(nameof(_main.CurrentDepartment));
                        _main.OnPropertyChanged(nameof(_main.Departments));
                        _main.AddNotification(new Notification("Name change success", Notification.APPROVE));
                    }
                }
                else
                    _main.AddNotification(
                        new Notification("ERROR! Department name cannot be empty. Please enter a name.",
                            Notification.ERROR), 3500);
            }
        }
    }
}