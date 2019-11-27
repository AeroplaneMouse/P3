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
using System.Windows.Controls;

namespace AMS.ViewModels
{
    public class AssetListViewModel : BaseViewModel
    {
        private IAssetListController _listController;
        private string _searchQuery;
        private TagHelper _tagHelper;
        private bool _isStrict = false;

        public ObservableCollection<Asset> Items { get; set; }
        public List<Asset> SelectedItems { get; set; } = new List<Asset>();
        public bool IsStrict { 
            get => _isStrict; 
            set { _isStrict = value; 
                SearchAssets(); 
                Console.WriteLine("Strict toggled: "+value.ToString());
            }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;

                if (!inTagMode && _searchQuery == "#")
                {
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
        public ITagable TagParent { get; set; }
        public bool inTagMode { get; set; } = false;
        public ObservableCollection<ITagable> AppliedTags { get; set; } = new ObservableCollection<ITagable>();


        #region Commands

        public ICommand AddNewCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ViewCommand { get; set; }
        public ICommand ViewWithParameterCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand RemoveTagCommand { get; set; }
        public ICommand RemoveBySelectionCommand { get; set; }
        public ICommand EditBySelectionCommand { get; set; }
        public ICommand AutoTagCommand { get; set; }
        public ICommand ClearInputCommand { get; set; }
        public ICommand EnterSuggestionListCommand { get; set; }

        #endregion

        #region Constructor

        public AssetListViewModel(IAssetListController listController, TagHelper tagHelper)
        {
            _listController = listController;
            Items = _listController.AssetList;

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

            EnterSuggestionListCommand = new RelayCommand<object>((parameter) => test(parameter));

            // Other functions
            SearchCommand = new RelayCommand(SearchAssets);
            ViewCommand = new RelayCommand(ViewAsset);
            ViewWithParameterCommand = new RelayCommand<object>(ViewAsset);
            RemoveTagCommand = new RelayCommand<object>((parameter) => 
            {
                ITagable tag = parameter as ITagable;
                _tagHelper.RemoveTag(tag);
                AppliedTags = _tagHelper.GetAppliedTags(true);
                SearchAssets();
            });

            AutoTagCommand = new RelayCommand<object>((parameter) => AutoTag(parameter as ITagable));
            ClearInputCommand = new RelayCommand(ClearInput);
        }

        private void test(object parameter)
        {
            var list = parameter as ListBox;
            Keyboard.Focus(list);
            var item = list.SelectedItem = TagSearchSuggestions[0];



            //Keyboard.Focus(item);
            //throw new NotImplementedException();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Changes the content to AssetEditor with the selected asset
        /// if asset is null, a new asset will be created.
        /// </summary>
        private void EditAsset(Asset asset)
        {
            Features.Navigate.To(Features.Create.AssetEditor(asset));
        }

        private void EditBySelection()
        {
            // Only ONE item can be edited
            if (SelectedItems.Count == 1)
                EditAsset(SelectedItems.First());
            else
                Features.AddNotification(new Notification("Please select only one asset.", Notification.ERROR), 3500);
        }

        /// <summary>
        /// Searches the list for Assets matching the searchQuery
        /// </summary>
        private void SearchAssets()
        {
            if (inTagMode)
            {
                if (SearchQuery == "" && _tagHelper.IsParentSet()){
                    _tagHelper.ApplyTag(_tagHelper.GetParent());
                    _tagHelper.Parent(null);
                    CurrentGroup = "#";
                    AppliedTags = _tagHelper.GetAppliedTags(true);
                }
                
                AutoTag();
            }
            else
            {
                if (SearchQuery == null)
                    return;
            }
            
            RefreshList();
        }

        private void RefreshList()
        {
            _listController.Search(inTagMode ? "" : SearchQuery, _tagHelper.GetAppliedTagIds(typeof(Tag)), _tagHelper.GetAppliedTagIds(typeof(User)), _isStrict);
            Items = _listController.AssetList;
        }

        private void AutoTag(ITagable input = null)
        {
            if (!inTagMode)
                    return;

            // Use the given input
            ITagable tag = input;

            // If the input is null, use the suggestion if possible
            if (tag == null && TagSearchSuggestions != null && TagSearchSuggestions.Count > 0)
                tag = TagSearchSuggestions[0];

            // If there is something to apply, do it
            if (tag != null)
            {
                if (_tagHelper.IsParentSet() || (tag.ChildrenCount == 0 && tag.TagId != 1)){
                    _tagHelper.ApplyTag(tag);
                    AppliedTags = _tagHelper.GetAppliedTags(true);
                    TagSearchProcess();
                }
                else
                {
                    // So we need to switch to a group of tags.
                    Tag taggedItem = (Tag)tag;
                    _tagHelper.Parent(taggedItem);
                    CurrentGroup = "#"+taggedItem.Name;
                }

                RefreshList();
                SearchQuery = "";
            }
        }

        private void ClearInput()
        {
            if (!inTagMode)
                return;

            if (_tagHelper.IsParentSet()){
                _tagHelper.Parent(null);
                CurrentGroup = "#";
                SearchQuery = "";
                TagSearchProcess();
            }else{
                LeavingTagMode();
            }
        }

        private void EnteringTagMode()
        {
            inTagMode = true;
            CurrentGroup = "#";
            CurrentGroupVisibility = Visibility.Visible;
            SearchQuery = "";
        }

        private void TagSearchProcess()
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
                Features.Navigate.To(Features.Create.AssetPresenter(SelectedItems.First(), _listController.GetTags(SelectedItems.First())));
            else
                Features.AddNotification(new Notification("Can only view one asset", Notification.ERROR));
        }

        private void ViewAsset(object asset)
        {
            Features.Navigate.To(Features.Create.AssetPresenter(asset as Asset, _listController.GetTags(asset as Asset)));
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
                    Features.AddNotification(new Notification(asset.Name + " has been removed from the system", Notification.INFO));
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
