using Asset_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels.Commands
{
    class AddFieldCommand : ICommand
    {
        private FieldsController _viewModel;
        public event EventHandler CanExecuteChanged;

        public AddFieldCommand(FieldsController viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            string fieldToAdd = parameter.ToString();

            // Getting label, default value and is required
            List<string> promptResults = _viewModel.PromptManager(fieldToAdd, out bool required);

            // Adding the new field
            _viewModel.FieldsList.Add(new Field(promptResults[0], promptResults[1], 1, promptResults[1], required));
        }
    }
}
