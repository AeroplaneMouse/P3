using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using AMS.Controllers;
using AMS.Controllers.Interfaces;
using AMS.Events;
using AMS.Models;
using AMS.ViewModels.Base;
using AMS.Views.Prompts;

namespace AMS.ViewModels
{
    public class TagListViewModel : BaseViewModel
    {
        private readonly ITagListController _tagListController;
        private readonly ITagController _tagController;
        private string _searchQuery = "";

        public List<Tag> Tags { get; set; }
        public Tag SelectedItem { get; set; }

        public string CurrentDepartment => "(" + Features.GetCurrentDepartment().Name + ")";
        public ICommand RemoveCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand AddNewCommand { get; set; }
        public ICommand SearchCommand { get; set; }

        public Visibility RemoveSelectedVisibility { get; set; } = Visibility.Collapsed;

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                Search();
            }
        }
        
        public TagListViewModel(ITagListController controller, ITagController tagController)
        {
            _tagListController = controller;
            _tagController = tagController;

            RemoveCommand = new RelayCommand(RemoveTag);
            AddNewCommand = new RelayCommand(() => Features.Navigate.To(Features.Create.TagEditor(null)));
            SearchCommand = new RelayCommand(() => Search());
            EditCommand = new RelayCommand(() => {
                if (SelectedItem != null)
                    Features.Navigate.To(Features.Create.TagEditor(_tagListController.getTag(SelectedItem.ID)));
            });
            
            Search();
        }

        public override void UpdateOnFocus()
        {
            OnPropertyChanged(nameof(CurrentDepartment));
            Search();
            OnPropertyChanged(nameof(Tags));
        }

        private void RemoveTag()
        {
            string message = String.Empty;

            // Get complete tag from database
            _tagController.ControlledTag = _tagController.GetTagById(SelectedItem.ID);
            
            // Check if parent
            if (_tagController.ParentId == 0 && _tagController.ControlledTag.NumberOfChildren > 0)
            {
                message = "You are about to remove a parent tag!\n"
                        + $"There are { _tagController.ControlledTag.NumberOfChildren } children attached to this parent.\n\n"
                        + "- Remove parent and all children\n"
                        + "- Remove parent and convert children to parents";

                List<string> buttons = new List<string>();
                buttons.Add("Remove all");
                buttons.Add("Remove parent");

                Features.DisplayPrompt(new Views.Prompts.ExpandedConfirm(message, buttons, (sender, e) =>
                {
                    if (e is ExpandedPromptEventArgs args)
                    {
                        string extraMessage = $"{ _tagController.Name } has been removed";
                        bool actionSuccess;
                        if (args.ButtonNumber == 0)
                        {
                            actionSuccess = _tagController.Remove(removeChildren: true);
                            extraMessage += $" aswell as { _tagController.ControlledTag.NumberOfChildren } children";
                        }
                        else
                            actionSuccess = _tagController.Remove();

                        if (!actionSuccess)
                            extraMessage = "Error! Unable to remove tag(s).";

                        Features.AddNotification(new Notification(extraMessage, background: actionSuccess ? Notification.APPROVE : Notification.ERROR), displayTime: 4000);
                        UpdateOnFocus();
                    }
                }));
            }

            else
            {
                if (_tagController.Id == 1)
                {
                    Features.AddNotification(new Notification($"{_tagController.Name} cannot be removed, it is essential", Notification.WARNING));
                }
                else
                {
                    Features.DisplayPrompt(new Views.Prompts.Confirm(
                    "You are about to remove a tag which cannot be UNDONE!\n"
                    + "Are you sure?\n"
                    + $"Tag: { _tagController.Name }", (sender, e) =>
                    {
                        _tagController.Remove();
                        UpdateOnFocus();
                        Features.AddNotification(new Notification($"{ _tagController.Name } has been remove.", background: Notification.APPROVE));
                    }
                }));
            }
        }

        private void Search()
        {
            _tagListController.GetTreeviewData(_searchQuery);
            Tags = _tagListController.TagsList;
        }
    }
}