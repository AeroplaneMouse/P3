using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Asset_Management_System.Models;
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
            

            RemoveFromTagList(_viewModel.FieldsList,tagId);
            RemoveFromTagList(_viewModel.HiddenFields,tagId);
            
            if (removeList.Count > 0)
            {
                DeleteFields();
            }
        }

        private void DeleteFields()
        {
            foreach (var currentShownField in removeList)
            {
                if (_viewModel.FieldsList.SingleOrDefault(shownField =>
                        shownField.Field.Equals(currentShownField.Field)) != null)
                {
                    _viewModel.FieldsList.Remove(_viewModel.FieldsList.SingleOrDefault(shownField =>
                        shownField.Field.Equals(currentShownField.Field)));
                }
                
                if (_viewModel.HiddenFields.SingleOrDefault(shownField =>
                        shownField.Field.Equals(currentShownField.Field)) != null)
                {
                    _viewModel.HiddenFields.Remove(_viewModel.HiddenFields.SingleOrDefault(shownField =>
                        shownField.Field.Equals(currentShownField.Field)));
                }
            }
        }

        private void RemoveFromTagList(ObservableCollection<ShownField> inputList,ulong tagId)
        {
            //Remove references from a field, to the input tag.
            foreach (var currentShownField in inputList)
            {
                if (currentShownField.FieldTags.Remove(
                    currentShownField.FieldTags.SingleOrDefault(tag => tag.ID == tagId)))
                {
                    if (currentShownField.FieldTags.Count == 0)
                    {
                        removeList.Add(currentShownField);
                    }
                }
            }
        }
    }
}