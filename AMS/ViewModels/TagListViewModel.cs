using System.Collections.Generic;
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
    public class TagListViewModel
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
            RemoveCommand = new Base.RelayCommand(() => Features.DisplayPrompt(new Confirm("Delete the selected tag, cannot be recovered?",RemoveTag)));
            EditCommand = new Base.RelayCommand(() => Features.NavigatePage(PageMaker.CreateTagEditor(SelectedItem)));
            AddNewCommand = new Base.RelayCommand(() => Features.NavigatePage(PageMaker.CreateTagEditor(null)));


            foreach (var tag in Tags)
            {
                if (tag.NumOfChildren <= 0) continue;
                List<Tag> offspring = _tagListController.GetChildTags(tag.ID);
                tag.Children = new List<ITagable>();
                tag.Children.AddRange(offspring);
            }
        }

        private void RemoveTag(object sender, PromptEventArgs e)
        {
            // If the prompt returns true, delete the item
            if (e.Result)
            {
                _tagListController.Remove(SelectedItem);
            }
        }
    }
}