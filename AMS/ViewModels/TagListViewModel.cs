using System.Collections.Generic;
using System.Windows.Input;
using AMS.Controllers.Interfaces;
using AMS.Events;
using AMS.Models;
using AMS.ViewModels.Base;
using AMS.Views.Prompts;

namespace AMS.ViewModels
{
    public class TagListViewModel : BaseViewModel
    {
        public List<Tag> Tags { get; set; }
        private readonly ITagListController _tagListController;
        private string _searchQuery = "";
        public Tag SelectedItem { get; set; }
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
        
        public TagListViewModel(ITagListController controller)
        {
            _tagListController = controller;

            RemoveCommand = new RelayCommand(() => Features.DisplayPrompt(new Confirm("Deleting selected tag. This action cannot be undone. Proceed?", RemoveTag)));
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
            Search();
            OnPropertyChanged(nameof(Tags));
        }

        private void RemoveTag(object sender, PromptEventArgs e)
        {
            // If the prompt returns true, delete the item
            if (e.Result)
            {
                _tagListController.Remove(SelectedItem);
            }

            Search();
            OnPropertyChanged(nameof(Tags));
        }

        private void Search()
        {
            _tagListController.GetTreeviewData(_searchQuery);
            Tags = _tagListController.TagsList;
        }
    }
}