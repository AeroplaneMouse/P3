using System;
using System.Linq;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels.Commands
{
    public class UntagTagCommand : ICommand
    {
        private AssetManagerViewModel _viewModel;
        public event EventHandler CanExecuteChanged;

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

            // Find field by ID, then remove it.
            _viewModel.CurrentlyAddedTags.Remove(_viewModel.CurrentlyAddedTags.Single(s => s.ID == tagId));

            foreach (var currentFieldList in _viewModel.FieldsList)
            {
                currentFieldList.FieldTags.Remove(currentFieldList.FieldTags.Single(s => s.ID == tagId));
            }
        }
    }
}