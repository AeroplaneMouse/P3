﻿using System;
using AMS.Views;
using AMS.Models;
using System.Linq;
using AMS.Helpers;
using AMS.Interfaces;
using System.Windows;
using AMS.Controllers;
using AMS.ViewModels.Base;
using System.Windows.Input;
using AMS.Database.Repositories;
using AMS.Controllers.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace AMS.ViewModels
{
    public class AssetListViewModel : BaseViewModel
    {
        private IAssetListController _listController;
        private string _searchQuery = String.Empty;
        private TagHelper _tagHelper;
        private bool _isStrict = true;
        private bool _searchInFields = false;
        private int _tabIndex = 0;

        public ObservableCollection<Asset> Items => new ObservableCollection<Asset>(_listController.AssetList);
        public List<Asset> SelectedItems { get; set; } = new List<Asset>();
        public bool IsStrict { 
            get => _isStrict; 
            set {
                _isStrict = value;
                RefreshList();
            }
        }

        public bool SearchInFields
        {
            get => _searchInFields;
            set
            {
                _searchInFields = value;
                RefreshList();
            }
        }

        public bool CheckAll { get; set; }
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;

                if (!inTagMode && _searchQuery == "#")
                {
                    EnterTagMode();
                }

                if (inTagMode && !_searchQuery.EndsWith(' '))
                {
                    UpdateTagSuggestions();
                }

                if (!inTagMode)
                    RefreshList();
            }
        }
        public string CurrentGroup { get; set; }
        public Visibility CurrentGroupVisibility { get; set; } = Visibility.Collapsed;
        public Visibility TagSuggestionsVisibility { get; set; } = Visibility.Collapsed;
        public Visibility SingleSelected { get; set; } = Visibility.Collapsed;
        public Visibility MultipleSelected { get; set; } = Visibility.Collapsed;
        public bool TagSuggestionIsOpen { get; set; } = false;
        public ObservableCollection<ITagable> TagSearchSuggestions { get; set; } = new ObservableCollection<ITagable>();
        public ITagable TagParent { get; set; }
        public bool inTagMode { get; set; } = false;
        public ObservableCollection<ITagable> AppliedTags { get; set; } = new ObservableCollection<ITagable>();

        public ICommand AddNewCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        public ICommand ApplyTagOrEnterParentCommand { get; set; }
        public ICommand ViewCommand { get; set; }
        public ICommand ViewWithParameterCommand { get; set; }
        public ICommand RemoveTagCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand RemoveBySelectionCommand { get; set; }
        public ICommand EditBySelectionCommand { get; set; }
        public ICommand InsertNextOrSelectedSuggestionCommand { get; set; }
        public ICommand ClearInputCommand { get; set; }
        public ICommand EnterSuggestionListCommand { get; set; }
        public ICommand CheckAllChangedCommand { get; set; }
        public ICommand SearchCommand { get; set; }

        public AssetListViewModel(IAssetListController listController, TagHelper tagHelper)
        {
            _listController = listController;

            _tagHelper = tagHelper;
            _tagHelper.CanApplyParentTags = true;

            // Initialize commands

            // Admin only functions
            if (Features.GetCurrentSession().IsAdmin())
            {
                AddNewCommand = new RelayCommand(() => EditAsset(null));
                EditCommand = new RelayCommand<object>((parameter) => EditAsset(parameter as Asset));
                RemoveCommand = new RelayCommand<object>((parameter) => RemoveAsset(parameter as Asset));
                RemoveBySelectionCommand = new RelayCommand(RemoveSelected);
                EditBySelectionCommand = new RelayCommand(EditBySelection);
                PrintCommand = new RelayCommand(Export);
            }

            EnterSuggestionListCommand = new RelayCommand<object>((parameter) => FocusSuggestionList(parameter));

            // Other functions
            ApplyTagOrEnterParentCommand = new RelayCommand(ApplyTagOrEnterParent);
            SearchCommand = new RelayCommand(RefreshList);
            ViewCommand = new RelayCommand(ViewAsset);
            ViewWithParameterCommand = new RelayCommand<object>(ViewAsset);
            
            RemoveTagCommand = new RelayCommand<object>((parameter) => {
                ITagable tag = parameter as ITagable;
                _tagHelper.RemoveTag(tag);
                AppliedTags = _tagHelper.GetAppliedTags(true);
                RefreshList();
            });

            InsertNextOrSelectedSuggestionCommand = new RelayCommand<object>((parameter) => InsertNextOrSelectedSuggestion(parameter));
            ClearInputCommand = new RelayCommand(ClearInput);
            CheckAllChangedCommand = new RelayCommand<object>((parameter) => CheckAllChanged(parameter as ListView));
        }
        
        #region Methods

        /// <summary>
        /// Reflects the updates in the in data in the view
        /// </summary>
        public override void UpdateOnFocus()
        {
            OnPropertyChanged(nameof(Items));
            OnPropertyChanged(nameof(SelectedItems));
            OnPropertyChanged(nameof(IsStrict));
            OnPropertyChanged(nameof(SearchQuery));
            OnPropertyChanged(nameof(AppliedTags));
        }

        /// <summary>
        /// Checks or unchecks all checkboxes in the given list, based on the current status
        /// </summary>
        /// <param name="list"></param>
        private void CheckAllChanged(ListView list)
        {
            if (SelectedItems.Count == 0)
            {
                // None selected. Select all.
                CheckAll = true;
                list.SelectAll();
            }
            else if (SelectedItems.Count <= Items.Count)
            {
                // Some selected or all selected. Unselect all
                CheckAll = false;
                list.UnselectAll();
            }
        }

        /// <summary>
        /// Focuses the suggestion list of the seach field
        /// </summary>
        /// <param name="parameter"></param>
        private void FocusSuggestionList(object parameter)
        {
            var list = parameter as ListBox;
            Keyboard.Focus(list);
        }

        /// <summary>
        /// Changes the content to AssetEditor with the selected asset
        /// if asset is null, a new asset will be created.
        /// </summary>
        private void EditAsset(Asset asset)
        {
            Features.Navigate.To(Features.Create.AssetEditor(asset));
            OnPropertyChanged(nameof(Items));
        }

        private void EditBySelection()
        {
            // Only ONE item can be edited
            if (SelectedItems.Count == 1)
            {
                EditAsset(SelectedItems.First());
                OnPropertyChanged(nameof(Items));
            }
            else
                Features.AddNotification(new Notification("Please select only one asset.", Notification.ERROR), 3500);
        }

        /// <summary>
        /// Inserts the next suggestion into the search query, or the selected element from the list
        /// </summary>
        /// <param name="input">The selected element (optional)</param>
        private void InsertNextOrSelectedSuggestion(object input = null)
        {
            if (!inTagMode)
                return;

            // If the input is null, use the suggestion if possible
            if (input != null)
            {
                ITagable tag;

                if (input is TextBlock textBlock)
                {
                    tag = TagSearchSuggestions.SingleOrDefault(t => t.TagLabel == textBlock.Text);
                }
                else
                {
                    tag = (ITagable)input;
                }

                if (_tagHelper.IsParentSet() || (tag.ChildrenCount == 0 && tag.TagId != 1))
                {
                    if(tag != null) {
                        _tagHelper.AddTag(tag);
                    }
                    else if (_tagHelper.GetParent() != null)
                    {
                        _tagHelper.AddTag(_tagHelper.GetParent());
                    }

                    AppliedTags = _tagHelper.GetAppliedTags(true);
                    RefreshList();
                    UpdateTagSuggestions();
                }
                else
                {
                    // So we need to switch to a group of tags.
                    Tag taggedItem = (Tag)tag;
                    _tagHelper.SetParent(taggedItem);
                    CurrentGroup = "#" + taggedItem.Name;
                    UpdateTagSuggestions();
                }
                SearchQuery = "";
            }
            else if (TagSearchSuggestions != null && TagSearchSuggestions.Count > 0)
            {
                if (!(_tabIndex <= TagSearchSuggestions.Count() - 1))
                {
                    _tabIndex = 0;
                }
                SearchQuery = TagSearchSuggestions[_tabIndex].TagLabel + ' ';
                _tabIndex++;
            }
        }

        /// <summary>
        /// Applies the tag or enters the tag, if it is a parent
        /// </summary>
        private void ApplyTagOrEnterParent()
        {
            if (!inTagMode)
                return;

            ITagable tag = TagSearchSuggestions.SingleOrDefault<ITagable>(t => t.TagLabel == SearchQuery.Trim(' '));
            if (tag != null)
            {
                if (tag.ParentId == 0 && (tag.TagId == 1 || tag.ChildrenCount > 0))
                {
                    _tagHelper.SetParent((Tag)tag);
                    CurrentGroup = "#" + tag.TagLabel;
                }
                else
                {
                    _tagHelper.AddTag(tag);
                    AppliedTags = _tagHelper.GetAppliedTags(true);
                }
                SearchQuery = "";
                _tabIndex = 0;
            }
            else
            {
                if (_tagHelper.IsParentSet() && SearchQuery == "")
                {
                    _tagHelper.AddTag(_tagHelper.GetParent());
                    AppliedTags = _tagHelper.GetAppliedTags(true);
                    ClearInput();
                    _tabIndex = 0;
                }
                Features.AddNotification(new Notification($"{ SearchQuery } is not a tag. To use it, you must first create a tag called { SearchQuery }.",
                        background: Notification.WARNING),
                    displayTime: 3500);
            }

            RefreshList();
        }

        /// <summary>
        /// Refreshes the asset list, to accommodate for new search query and tags
        /// </summary>
        private void RefreshList()
        {
            _listController.Search(inTagMode ? "" : SearchQuery, _tagHelper.GetAppliedTagIds(typeof(Tag)), _tagHelper.GetAppliedTagIds(typeof(User)), _isStrict, _searchInFields);
            OnPropertyChanged(nameof(Items));
        }

        /// <summary>
        /// Clears the search query
        /// </summary>
        private void ClearInput()
        {
            if (!inTagMode)
            {
                SearchQuery = "";
                return;
            }

            if (_tagHelper.IsParentSet())
            {
                _tagHelper.SetParent(null);
                CurrentGroup = "#";
                SearchQuery = "";
                UpdateTagSuggestions();
            }

            else
            {
                LeaveTagMode();
            }
        }
        /// <summary>
        /// Enters the tag mode of the search field
        /// </summary>
        private void EnterTagMode()
        {
            inTagMode = true;
            CurrentGroup = "#";
            CurrentGroupVisibility = Visibility.Visible;
            SearchQuery = "";
        }

        /// <summary>
        /// Updates the tag suggestions
        /// </summary>
        private void UpdateTagSuggestions()
        {
            TagSearchSuggestions = new ObservableCollection<ITagable>(_tagHelper.Suggest(_searchQuery));

            if (_tagHelper.HasSuggestions())
            {
                TagSuggestionsVisibility = Visibility.Visible;
                TagSuggestionIsOpen = true;
            }
            else
            {
                TagSuggestionsVisibility = Visibility.Collapsed;
                TagSuggestionIsOpen = false;
            }
        }

        /// <summary>
        /// Leaves the tag mode in the search field, and enters the 
        /// </summary>
        private void LeaveTagMode()
        {
            inTagMode = false;
            CurrentGroup = "";
            SearchQuery = "";
            TagSuggestionsVisibility = Visibility.Collapsed;
            TagSuggestionIsOpen = false;
            CurrentGroupVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Changes the content to ViewAsset with the selected asset
        /// </summary>
        private void ViewAsset()
        {
            if (SelectedItems.Count == 1)
                Features.Navigate.To(Features.Create.AssetPresenter(SelectedItems.First(), _listController.GetTags(SelectedItems.First())));
            else
                Features.AddNotification(new Notification("Can only view one asset", Notification.ERROR));
        }

        /// <summary>
        /// Changes the content to ViewAsset based on input asset
        /// </summary>
        /// <param name="asset">The asset to be viewed</param>
        private void ViewAsset(object asset)
        {
            Features.Navigate.To(Features.Create.AssetPresenter(asset as Asset, _listController.GetTags(asset as Asset)));
        }

        /// <summary>
        /// Remove an asset with prompt
        /// </summary>
        /// <param name="asset"></param>
        private void RemoveAsset(Asset asset)
        {
            if (asset != null)
            {
                Features.DisplayPrompt(new Views.Prompts.Confirm($"Are you sure you want to delete\n{ asset.Name }?", (sender, e) =>
                {
                    if (e.Result)
                    {
                        _listController.Remove(asset);
                        Features.AddNotification(new Notification($"{ asset.Name } has been removed", Notification.APPROVE));
                        ApplyTagOrEnterParent();
                        RefreshList();
                    }
                }));
            }
        }

        /// <summary>
        /// Removes all the selected items
        /// </summary>
        private void RemoveSelected()
        {
            if (SelectedItems.Count > 0)
            {
                // Prompt user for approval for the removal of x assets 
                string message = $"Are you sure you want to remove " +
                                 $"{ ((SelectedItems.Count == 1) ? SelectedItems[0].Name : (SelectedItems.Count.ToString()) + " assets")}?";

                Features.DisplayPrompt(new Views.Prompts.Confirm(message, (sender, e) =>
                {
                    // Check if the user approved
                    if (e.Result)
                    {
                        // Move selected items to new list
                        // Hvorfor flyttes de til en ny liste?
                        List<Asset> items = new List<Asset>();
                        SelectedItems.ForEach(a => items.Add(a));
                        // Remove each asset
                        foreach (Asset asset in items)
                            _listController.Remove(asset);

                        Features.AddNotification(
                            new Notification($"" +
                            $"{ ((items.Count == 1) ? items[0].Name : (items.Count + " assets")) } " +
                            $"{ (items.Count > 1 ? "have" : "has") } been removed from the system",

                            Notification.INFO), 3000);
                        ApplyTagOrEnterParent();
                        OnPropertyChanged(nameof(Items));
                    }
                }));
            }
        }

        /// <summary>
        /// Exports selected assets to .csv file
        /// </summary>
        private void Export()
        {
            if (SelectedItems.Count > 0)
                // Export selected items
                _listController.Export(SelectedItems);
            else
                // Export all items found by search
                _listController.Export(Items.ToList());
        }

        #endregion
    }
}