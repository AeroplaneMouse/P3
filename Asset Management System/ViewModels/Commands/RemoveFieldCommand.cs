using Asset_Management_System.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Asset_Management_System.ViewModels.ViewModelHelper;

namespace Asset_Management_System.ViewModels.Commands
{
    class RemoveFieldCommand : ICommand
    {
        private ObjectViewModelHelper _viewModel;
        public event EventHandler CanExecuteChanged;

        public RemoveFieldCommand(ObjectViewModelHelper viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            string fieldId = parameter?.ToString() ?? throw new NullReferenceException("Input parameter == null");

            ShownField shownField = _viewModel.FieldsList.SingleOrDefault(s => s.Field.HashId == fieldId);
            if (shownField == null)
            {
                shownField = _viewModel.HiddenFields.SingleOrDefault(s => s.Field.HashId == fieldId);
            }
            // Find field by ID, then remove it.
            if (shownField.Field.IsHidden)
            {
                _viewModel.FieldsList.Add(shownField);
                _viewModel.HiddenFields.Remove(shownField);
                shownField.Field.IsHidden = false;
            }
            else
            {
                if (!shownField.Field.IsCustom)
                {
                    shownField.Field.IsHidden = true;
                    _viewModel.HiddenFields.Add(shownField);
                }
                _viewModel.FieldsList.Remove(shownField);

            }
            
            
        }
    }
}