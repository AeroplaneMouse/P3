using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Asset_Management_System.Resources.DataModels;
using Asset_Management_System.Services.Interfaces;
using System.Windows;
using Asset_Management_System.Functions;

namespace Asset_Management_System.ViewModels
{
    public class AssetsViewModel : ChangeableListPageViewModel<Asset>
    {
        private string _currentGroup = String.Empty;
        private string _searchQueryText = String.Empty;
        private bool IsTagMode = false;
        private bool _isStrict = false;
        private IAssetService _service;
        private IAssetRepository _rep;
        
        public int ViewType => 1;
        public int SelectedSuggestedIndex { get; set; }
        public Visibility IsCurrentGroupVisible { get; set; } = Visibility.Hidden;
        public ICommand DeleteCommand { get; set; }
        public ICommand EnterCommand { get; set; }
        public ICommand SelectTagCommand { get; set; }
        public bool IsStrict { get => _isStrict; set { _isStrict = value; Search(); } }

        public Tagging TheTagger { get; }

        public List<ITagable> Suggestions { get; set; } = new List<ITagable>();

        public string CurrentGroup
        {
            get => _currentGroup;
            set
            {
                _currentGroup = value;
                IsCurrentGroupVisible = (value == string.Empty ? Visibility.Hidden : Visibility.Visible);
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

        public AssetsViewModel(MainViewModel main, IAssetService assetService) : base(main, assetService)
        {
            DeleteCommand = new Base.RelayCommand(Delete);
            SelectTagCommand = new Base.RelayCommand(SelectSuggestedTag);
            EnterCommand = new Base.RelayCommand(Enter);
            TheTagger = new Tagging();
            _service = assetService;
            _rep = _service.GetSearchableRepository() as IAssetRepository;
        }

        // Add the currently selected tag from suggestions
        private void SelectSuggestedTag()
        {
            if (SelectedSuggestedIndex == -1 && Suggestions.Count > 0)
                SelectedSuggestedIndex = 0;

            SelectTag(Suggestions[SelectedSuggestedIndex]);
            Search();
            // TODO: Set focus to searchbar
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

        public override void PageLostFocus()
        {

        }

        private void Enter()
        {
            if (IsTagMode)
            {
                if (SearchQueryText == String.Empty && TheTagger.IsParentSet())
                    TheTagger.AddToQuery(TheTagger.GetParent());
                else if (Suggestions.Count > 0)
                    SelectTag(Suggestions.First());
            }
            Search();
        }

        // Search assets
        protected override void Search()
        {
            SearchList = _rep.Search(SearchQueryText,
                TheTagger.TaggedWith.OfType<Tag>().Select(t => t.ID).ToList(),
                TheTagger.TaggedWith.OfType<User>().Select(u => u.ID).ToList(),
                IsStrict);
        }

        // Add the given tag to the search query
        private void SelectTag(ITagable t)
        {
            if (TheTagger.IsParentSet())
            {
                TheTagger.AddToQuery(t);
                OnPropertyChanged(nameof(TheTagger.TaggedWith));
            }
            else
            {
                Tag tag = (Tag)t;
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
