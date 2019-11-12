using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Asset_Management_System.Models;
using Asset_Management_System.ViewModels.ViewModelHelper;

namespace Asset_Management_System.ViewModels.Commands
{
    public class RemoveTagFromSearchCommand : ICommand
    {
        private AssetsViewModel _viewModel;
        public event EventHandler CanExecuteChanged;

        public RemoveTagFromSearchCommand(AssetsViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ulong tagId = ulong.Parse(parameter?.ToString() ?? throw new NullReferenceException("Input parameter == null"));

            // Find Tag by ID, then remove it.
            _viewModel.Tags.RemoveFromQuery(_viewModel.Tags.AppliedTags.Single(tag => tag.TagId() == tagId));

            _viewModel.Search();
        }
    }
}