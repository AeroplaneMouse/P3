using Asset_Management_System.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels.Commands
{
    class RemoveFieldCommand : ICommand
    {
        private FieldsController _viewModel;
        public event EventHandler CanExecuteChanged;

        public RemoveFieldCommand(FieldsController viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            int fieldId = int.Parse(parameter.ToString());

            // Find field by ID, then remove it.
            foreach(Field field in _viewModel.FieldsList)
            {
                if (field.ID == fieldId)
                {
                    _viewModel.FieldsList.Remove(field);
                    return;
                }
            }
        }
    }
}
