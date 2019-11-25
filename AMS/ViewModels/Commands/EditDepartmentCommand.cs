using System;
using AMS.Events;
using AMS.Models;
using AMS.Views.Prompts;
using System.Windows.Input;
using AMS.Database.Repositories;

namespace AMS.ViewModels.Commands
{
    class EditDepartmentCommand : ICommand
    {
        private MainViewModel _main;
        private Department _department;

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
                Features.AddNotification(new Models.Notification(
                    $"An unknown occured. Unable to edit department with id: { parameter.ToString() }",
                    Models.Notification.ERROR));
                return;
            }

            // Validating id
            // TODO: Ingen new repositories!
            _department = new DepartmentRepository().GetById(id);
            if (_department != null)
                Features.DisplayPrompt(new TextInput("Enter new name", _department.Name, PromptElapsed));
            else
                Features.AddNotification(
                    new Notification("Editing department failed. Department not found!", Notification.ERROR),
                    3500);
        }

        private void PromptElapsed(object sender, PromptEventArgs e)
        {
            TextInputPromptEventArgs f = e as TextInputPromptEventArgs;

            if (f != null && f.Result)
            {
                if (f.Text != String.Empty)
                {
                    _department.Name = f.Text;

                    // TODO: Ingen new repositories!
                    if (new DepartmentRepository().Update(_department))
                    {
                        _main.CurrentDepartment = _department;
                        _main.OnPropertyChanged(nameof(_main.CurrentDepartment));
                        _main.OnPropertyChanged(nameof(_main.Departments));
                        Features.AddNotification(new Notification("Name change success", Notification.APPROVE));
                    }
                }
                else
                    Features.AddNotification(
                        new Notification("Department name cannot be empty. Please enter a name.", Notification.ERROR), 
                        3500);
            }
        }
    }
}