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
        private readonly Func<bool> _func;

        public event EventHandler CanExecuteChanged;

        public EditDepartmentCommand(MainViewModel main, Func<bool> func)
        {
            _main = main;
            _func = func;
        }

        public bool CanExecute(object parameter)
        {
            if (_func != null)
                return _func();

            return true;
        }

        /// <summary>
        /// Displays a prompt where the name of the given department can be changed
        /// </summary>
        /// <param name="parameter">The ID of the department that is to be edited</param>
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
                    Notification.ERROR));
                return;
            }

            // Validating id
            _department = Features.DepartmentRepository.GetById(id);
            if (_department != null)
            {
                Features.DisplayPrompt(new TextInput("Enter new name", _department.Name, PromptElapsed));
            }
                
            else
            {
                Features.AddNotification(new Notification("Editing department failed. Department not found!", Notification.ERROR), 3500);
            }
                
        }
        
        /// <summary>
        /// When the prompt displayed by <see cref="Execute(object)"/> is closed,
        /// implements the changes that the user chose
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PromptElapsed(object sender, PromptEventArgs e)
        {
            TextInputPromptEventArgs textEventArgs = e as TextInputPromptEventArgs;

            if (textEventArgs != null && textEventArgs.Result)
            {
                if (textEventArgs.Text != String.Empty)
                {
                    _department.Name = textEventArgs.Text;

                    // If the department was updated in the database succesfully, 
                    // change the current department
                    if (Features.DepartmentRepository.Update(_department))
                    {
                        _main.CurrentDepartment = _department;
                        _main.OnPropertyChanged(nameof(_main.CurrentDepartment));
                        _main.OnPropertyChanged(nameof(_main.Departments));
                        Features.AddNotification(new Notification("Name change success", Notification.APPROVE));
                    }
                }
                else
                {
                    Features.AddNotification(new Notification("Department name cannot be empty. Please enter a name.", Notification.ERROR), 3500);
                }
                    
            }
        }
    }
}