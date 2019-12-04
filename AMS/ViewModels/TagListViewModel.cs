using System;
using System.Collections.Generic;
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

        public string TitleText => "Tags - " + Features.GetCurrentDepartment().Name;
        public ICommand RemoveCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand AddNewCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        
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
            OnPropertyChanged(nameof(TitleText));
            Search();
            OnPropertyChanged(nameof(Tags));
        }

        private void RemoveTag()
        {
            string message = String.Empty;
            _tagController.ControlledTag = SelectedItem;

            // Check if parent
            if (_tagController.ParentID == 0 && _tagController.ControlledTag.ChildrenCount > 0)
            {
                message = "You are about to remove a parent tag!\n"
                        + $"There are { _tagController.ControlledTag.ChildrenCount } children attached to this parent.";

                List<string> buttons = new List<string>();
                buttons.Add("Remove parent and all children?");
                buttons.Add("Remove parent and convert children to parents?");

                Features.DisplayPrompt(new Views.Prompts.ExpandedConfirm(message, buttons, (sender, e) =>
                {
                    if (e is ExpandedPromptEventArgs args)
                    {
                        string extraMessage = String.Empty;
                        if (args.ButtonNumber == 0)
                        {
                            _tagController.RemoveChildren();
                            _tagController.Remove();
                            extraMessage = $" aswell as { _tagController.ControlledTag.ChildrenCount } children";
                        }
                        else
                            _tagController.Remove();
                        Features.AddNotification(new Notification($"{ _tagController.Name } has been removed{ extraMessage }.", background: Notification.APPROVE), displayTime: 4000);
                    }
                }));
            }
            else
            {
                Features.DisplayPrompt(new Views.Prompts.Confirm("You are about to remove a tag which cannot be UNDONE!\nAre you sure?", (sender, e) =>
                {
                    if (e.Result)
                    {
                        _tagController.Remove();
                        Features.AddNotification(new Notification($"{ _tagController.Name } has been remove.", background: Notification.APPROVE));
                    }
                }));
            }
            Search();
        }

        private void Search()
        {
            _tagListController.GetTreeviewData(_searchQuery);
            Tags = _tagListController.TagsList;
        }
    }
}