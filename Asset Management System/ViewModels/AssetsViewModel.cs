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
        public Visibility IsCurrentGroupVisible { get; set; } = Visibility.Hidden;
        public ICommand DeleteCommand { get; set; }
        public ICommand DownCommand { get; set; }
        public Tagging TheTagger { get; }

        public List<ITagable> Suggestions { get; set; } = new List<ITagable>();

        public string CurrentGroup
        {
            get => _currentGroup;
            set
            {
                _currentGroup = value;
                if (value == String.Empty)
                    IsCurrentGroupVisible = Visibility.Hidden;
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
            DownCommand = new Base.RelayCommand(FocusFirstSelection);
            TheTagger = new Tagging();
        }

        private void FocusFirstSelection()
        {
            
        }

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

        public override void PageFocus()
        {
            Search();
            TheTagger.Reload();
        }

        protected override void Search()
        {
            if (!IsTagMode)
                base.Search();
            else
            {
                Tag tag = (Tag)Suggestions.First();
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
                SearchQueryText = "";


                //TheTagger.AddToQuery(Suggestions.First());
                //Tag tag = Suggestions.Find(t => t.TagLabel == SearchQueryText)
            }
        }
        
    }
}