using System;
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

namespace AMS.ViewModels
{
    public class AssetListViewModel : BaseViewModel
    {
        private IAssetListController _listController;
        private string _searchQuery;
        private TagHelper _tagHelper;

        public ObservableCollection<Asset> Items { get; set; }
        public List<Asset> SelectedItems { get; set; } = new List<Asset>();

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;

                if (!inTagMode && _searchQuery == "#")
                {
                    inTagMode = true;
                    _searchQuery = "";
                    EnteringTagMode();
                }

                if (inTagMode)
                {
                    TagSearchProcess();
                }
            }
        }

        public string CurrentGroup { get; set; }
        public Visibility CurrentGroupVisibility { get; set; } = Visibility.Collapsed;
        public Visibility TagSuggestionsVisibility { get; set; } = Visibility.Collapsed;
        public Visibility SingleSelected { get; set; } = Visibility.Collapsed;
        public Visibility MultipleSelected { get; set; } = Visibility.Collapsed;
        public bool TagSuggestionIsOpen { get; set; } = false;
        public ObservableCollection<ITagable> TagSearchSuggestions { get; set; }
        public ITagable TagParent;
        public bool inTagMode = false;
        public ObservableCollection<ITagable> AppliedTags { get; set; } = new ObservableCollection<ITagable>();

        public ICommand AddNewCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ViewCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand RemoveTagCommand { get; set; }
        public ICommand RemoveBySelectionCommand { get; set; }
        public ICommand EditBySelectionCommand { get; set; }

        public ICommand AutoTagCommand { get; set; }
        public ICommand ClearInputCommand { get; set; }

        public AssetListViewModel(IAssetListController listController, TagHelper tagHelper)
        {
            _listController = listController;
            Items = _listController.AssetList;

            _tagHelper = tagHelper;
            _tagHelper.CanApplyParentTags = true;

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

            // Other functions
            SearchCommand = new RelayCommand(SearchAssets);
            ViewCommand = new RelayCommand(ViewAsset);
            RemoveTagCommand = new RelayCommand<object>((parameter) => 
            {
                ITagable tag = parameter as ITagable;
                _tagHelper.RemoveTag(tag);
                AppliedTags = _tagHelper.GetAppliedTags(true);
                SearchAssets();
                OnPropertyChanged(nameof(AppliedTags));
            });

            AutoTagCommand = new RelayCommand(AutoTag);
            ClearInputCommand = new RelayCommand(ClearInput);
        }

        /// <summary>
        /// Changes the content to AssetEditor with the selected asset
        /// if asset is null, a new asset will be created.
        /// </summary>
        private void EditAsset(Asset asset)
        {
            Features.NavigatePage(PageMaker.CreateAssetEditor(asset));
        }

        private void EditBySelection()
        {
            // Only ONE item can be edited
            if (SelectedItems.Count == 1)
                EditAsset(SelectedItems.First());
            else
                Features.AddNotification(new Notification("Error! Please select one and only one asset.", Notification.ERROR), 3500);
        }

        /// <summary>
        /// Searches the list for Assets matching the searchQuery
        /// </summary>
        private void SearchAssets()
        {
            Console.WriteLine("This is super stupid!");

            if (SearchQuery == null)
                return;

            if (!inTagMode)
            {
                _listController.Search(SearchQuery, _tagHelper.GetAppliedTagIds(typeof(Tag)), _tagHelper.GetAppliedTagIds(typeof(User)));
            }
            else
            {
                if (SearchQuery == "" && _tagHelper.IsParentSet())
                {
                    _tagHelper.ApplyTag(_tagHelper.GetParent());
                    _tagHelper.Parent(null);
                    CurrentGroup = "#";
                    AppliedTags = _tagHelper.GetAppliedTags(true);
                }

                _listController.Search("", _tagHelper.GetAppliedTagIds(typeof(Tag)), _tagHelper.GetAppliedTagIds(typeof(User)));
            }

            Items = _listController.AssetList;
            /*
            if (SearchQuery == null) return;
            _listController.Search(SearchQuery);
            Items = _listController.AssetList;
            */
        }

        private void AutoTag()
        {
            Console.WriteLine("Tap clicked!");
            try
            {
                if (!inTagMode)
                    return;

                if (TagSearchSuggestions != null && TagSearchSuggestions.Count > 0)
                {
                    ITagable tag = TagSearchSuggestions[0];

                    if (_tagHelper.IsParentSet())
                    {
                        try
                        {
                            _tagHelper.ApplyTag(tag);
                            AppliedTags = _tagHelper.GetAppliedTags(true);
                            TagSearchProcess();
                            SearchAssets();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    else
                    {
                        // Only tag can be parent
                        Tag taggedItem = (Tag)tag;
                    
                        if (taggedItem.NumOfChildren > 0 || tag.TagId == 1)
                        {
                            // So we need to switch to a group of tags.
                            _tagHelper.Parent(taggedItem);
                            CurrentGroup = "#"+taggedItem.Name;
                        }
                        else
                        {
                            _tagHelper.ApplyTag(tag);
                            AppliedTags = _tagHelper.GetAppliedTags(true);
                            TagSearchProcess();
                            SearchAssets();
                        }
                    }
                    
                    SearchQuery = "";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        private void ClearInput()
        {
            if (!inTagMode)
                return;

            if (_tagHelper.IsParentSet())
            {
                _tagHelper.Parent(null);
                CurrentGroup = "#";
                SearchQuery = "";
                TagSearchProcess();
            }
            else
            {
                LeavingTagMode();
            }
        }

        private void EnteringTagMode()
        {
            CurrentGroup = "#";
            CurrentGroupVisibility = Visibility.Visible;
        }

        private void TagSearchProcess()
        {
            try
            {
                //Console.WriteLine("Hest!");
                if (_searchQuery == "exit")
                {
                    LeavingTagMode();
                    
                }
                else
                {
                    try
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
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void LeavingTagMode()
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
            {
                Features.NavigatePage(PageMaker.CreateAssetPresenter(SelectedItems.First(), _listController.GetTags(SelectedItems.First())));
            }
            else
                Features.AddNotification(new Notification("Can only view one asset", Notification.ERROR));
        }

        /// <summary>
        /// Removes the given asset and displays a prompt
        /// </summary>
        /// <param name="asset"></param>
        private void RemoveAsset(Asset asset)
        {
            // Prompt user for confirmation of removal
            Features.DisplayPrompt(new Views.Prompts.Confirm($"Are you sure you want to remove { asset.Name }?", (sender, e) =>
            {
                if (e.Result)
                {
                    Features.AddNotification(new Notification("Asset " + asset.Name + " was deleted", Notification.INFO));
                    _listController.Remove(asset);
                }
            }));
        }

        /// <summary>
        /// Removes all the selected items
        /// </summary>
        private void RemoveSelected()
        {
            if (SelectedItems.Count > 0)
            {
                // Prompt user for approval for the removal of x assets 
                string message = $"Are you sure you want to remove { SelectedItems.Count } asset";
                message += SelectedItems.Count > 1 ? "s?" : "?";
                Features.DisplayPrompt(new Views.Prompts.Confirm(message, (sender, e) =>
                {
                    // Check if the user approved
                    if (e.Result)
                    {
                        // Move selected items to new list
                        List<Asset> items = new List<Asset>();
                        SelectedItems.ForEach(a => items.Add(a));
                        // Remove each asset
                        foreach (Asset asset in items)
                            _listController.Remove(asset);

                        Features.AddNotification(
                            new Notification($"{ items.Count } asset{ (items.Count > 1 ? "s" : "" ) } have been removed from the system", Notification.INFO),
                            3000);
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
    }
}
