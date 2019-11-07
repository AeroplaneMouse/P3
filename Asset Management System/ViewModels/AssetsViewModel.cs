using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using Asset_Management_System.Helpers;
using Asset_Management_System.Logging;
using Asset_Management_System.Resources.DataModels;
using System.Windows;
using Asset_Management_System.Functions;

namespace Asset_Management_System.ViewModels
{
    public class AssetsViewModel : ChangeableListPageViewModel<AssetRepository, Asset>
    {
        private MainViewModel _main;
        private string _currentGroup = String.Empty;
        private string _searchQueryText = String.Empty;
        private bool IsTagMode = false;

        public int ViewType => 1;
        public int SelectedSuggestedIndex { get; set; }
        public Visibility IsCurrentGroupVisible { get; set; } = Visibility.Visible;
        public ICommand DeleteCommand { get; set; }
        public ICommand SelectTagCommand { get; set; }

        public Tagging TheTagger { get; }

        public List<ITagable> Suggestions { get; set; } = new List<ITagable>();

        public string CurrentGroup
        {
            get => _currentGroup;
            set
            {
                _currentGroup = value;
                if (value == String.Empty)
                    IsCurrentGroupVisible = Visibility.Visible;
                else
                    IsCurrentGroupVisible = Visibility.Visible;
            }
        }

        public new string SearchQueryText
        {
            get => _searchQueryText;
            set
            {
                _searchQueryText = value;
                if (value == "#" && !IsTagMode)
                {
                    _searchQueryText = "";
                    CurrentGroup = "#";
                    IsTagMode = true;
                    Suggestions = TheTagger.Suggest(_searchQueryText);
                }
                else if (IsTagMode)
                {
                    Suggestions = TheTagger.Suggest(_searchQueryText);
                }
            }
        }

        public AssetsViewModel(MainViewModel main, ListPageType pageType) : base(main, pageType)
        {
            DeleteCommand = new Base.RelayCommand(Delete);
            SelectTagCommand = new Base.RelayCommand(SelectSuggestedTag);
            TheTagger = new Tagging();
        }

        // Add the currently selected tag from suggestions
        private void SelectSuggestedTag()
        {
            if (SelectedSuggestedIndex == -1 && Suggestions.Count > 0)
                SelectedSuggestedIndex = 0;

            SelectTag((Tag)Suggestions[SelectedSuggestedIndex]);
        }

        // Exit parent tag or tagmode
        private void Delete()
        {
            if (CurrentGroup != String.Empty)
            {
                if (CurrentGroup == "#")
                {
                    CurrentGroup = String.Empty;
                    Suggestions = null;
                    IsTagMode = false;
                }
                else
                {
                    CurrentGroup = "#";
                    SearchQueryText = "";
                    TheTagger.Parent(null);
                    Suggestions = TheTagger.Suggest(_searchQueryText);
                }
            }
        }

        // Called when the page retrieves focus again.
        public override void PageFocus()
        {
            Search();
            TheTagger.Reload();
        }

        // Search assets
        protected override void Search()
        {
            if (!IsTagMode)
                base.Search();
            else
            {
                if (SearchQueryText == String.Empty && TheTagger.IsParentSet())
                    TheTagger.AddToQuery(TheTagger.GetParent());
                else
                {
                    Tag tag = (Tag)Suggestions.First();
                    SelectTag(tag);
                }
            }
        }

        // Add the given tag to the search query
        private void SelectTag(Tag tag)
        {
            if (TheTagger.IsParentSet())
            {
                TheTagger.AddToQuery(tag);
                OnPropertyChanged(nameof(TheTagger.TaggedWith));
            }
            else
            {
                TheTagger.Parent(tag);
                CurrentGroup = tag.Name;
            }

            if (SearchQueryText == String.Empty)
                Suggestions = TheTagger.Suggest(SearchQueryText);
            else    
                SearchQueryText = "";
        }
    }
}