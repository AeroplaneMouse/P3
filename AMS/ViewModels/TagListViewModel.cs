using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using AMS.Controllers;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Events;
using AMS.Helpers;
using AMS.Interfaces;
using AMS.Models;
using AMS.Views;
using AMS.Views.Prompts;

namespace AMS.ViewModels
{
    public class TagListViewModel : Base.BaseViewModel
    {
        public List<Tag> Tags { get; set; }

        private readonly ITagListController _tagListController;
        
        public Tag SelectedItem { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand AddNewCommand { get; set; }
        public TagListViewModel(ITagListController controller)
        {
            _tagListController = controller;

            Tags = _tagListController.TagsList;
            Tags.AddRange(_tagListController.GetParentTags());

            RemoveCommand = new Base.RelayCommand(RemoveTag);
            EditCommand = new Base.RelayCommand(() => {
                if (SelectedItem != null)
                    Features.Navigate.To(Features.Create.TagEditor(SelectedItem));
            });
            AddNewCommand = new Base.RelayCommand(() => Features.Navigate.To(Features.Create.TagEditor(null)));
            
            foreach (var tag in Tags)
            {
                if (tag.NumOfChildren <= 0) continue;
                List<Tag> children = _tagListController.GetChildTags(tag.ID);
                tag.Children = new List<ITagable>();
                tag.Children.AddRange(children);
            }
        }

        public override void UpdateOnFocus()
        {
            OnPropertyChanged(nameof(Tags));
        }

        private void RemoveTag()
        {
            if (SelectedItem != null)
            {
                if (SelectedItem.ID != 1)
                {
                    Features.DisplayPrompt(new Confirm("Deleting selected tag. This action cannot be undone. Proceed?", (object sender, PromptEventArgs e) =>
                    {
                        // If the prompt returns true, delete the item
                        if (e.Result)
                        {
                            _tagListController.Remove(SelectedItem);
                        }
                    }));

                    OnPropertyChanged(nameof(Tags));
                }

                else
                {
                    Features.AddNotification(new Notification($"{SelectedItem.Name} tag is essential and cannot be deleted", Notification.WARNING));
                }
            }
        }
    }
}