using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Asset_Management_System.ViewModels.ViewModelHelper;

namespace Asset_Management_System.ViewModels.Commands
{
    public class UntagTagCommand : ICommand
    {
        private AssetManagerViewModel _viewModel;
        public event EventHandler CanExecuteChanged;
        
        List<ShownField> removeList = new List<ShownField>();

        public UntagTagCommand(AssetManagerViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ulong tagId =
                ulong.Parse(parameter?.ToString() ?? throw new NullReferenceException("Input parameter == null"));
            
            

            // Find Tag by ID, then remove it.
            _viewModel.CurrentlyAddedTags.Remove(_viewModel.CurrentlyAddedTags.Single(tag => tag.ID == tagId));

            
            // Removes tags from the fields where it is referenced.
            foreach (var currentShownField in _viewModel.FieldsList)
            {
                currentShownField.FieldTags.Remove(currentShownField.FieldTags.SingleOrDefault(tag => tag.ID == tagId));
                if (currentShownField.FieldTags.Count == 0 && !currentShownField.Field.IsCustom)
                {
                    removeList.Add(currentShownField);
                }
            }

            if (removeList.Count > 0)
            {
                DeleteAssets();
            }
            
        }

        private void DeleteAssets()
        {
            foreach (var currentShownField in removeList)
            {
                _viewModel.FieldsList.Remove(currentShownField);
            }
        }
    }
}