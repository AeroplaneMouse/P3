using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using AMS.Views;
using AMS.Controllers;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories;
using AMS.Helpers;
using AMS.Models;
using AMS.ViewModels.Base;
using AMS.Interfaces;

namespace AMS.ViewModels
{
    public class AssetListViewModel : BaseViewModel
    {
        private IAssetListController _listController;
        private MainViewModel _main;
        private string _searchQuery;
        private TagHelper _tagHelper;

        public ObservableCollection<Asset> Items { get; set; }
        public int SelectedItemIndex { get; set; }

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
        public ICommand RemoveMultipleCommand { get; set; }

        public ICommand AutoTagCommand { get; set; }
        public ICommand ClearInputCommand { get; set; }

        public AssetListViewModel(MainViewModel main, IAssetListController listController)
        {
            _main = main;
            _listController = listController;
            Items = _listController.AssetList;
            
            AddNewCommand = new RelayCommand(AddNewAsset);
            EditCommand = new RelayCommand<object>((parameter) => EditAsset(parameter as Asset));
            PrintCommand = new RelayCommand(Export);
            SearchCommand = new RelayCommand(SearchAssets);
            ViewCommand = new RelayCommand(ViewAsset);
            RemoveCommand = new RelayCommand<object>((parameter) => RemoveAsset(parameter as Asset));
            RemoveMultipleCommand = new RelayCommand<object>((parameter) => RemoveAssets(parameter as List<Asset>));
            AutoTagCommand = new RelayCommand(AutoTag);
            ClearInputCommand = new RelayCommand(ClearInput);
        }

        private void RemoveAssets(List<Asset> list)
        {
            if (list != null)
            {
                // Prompt user for approval for the removal of x assets 
                string message = $"Are you sure you want to remove { list.Count } asset";
                message += list.Count > 1 ? "s?" : "?";
                _main.DisplayPrompt(new Views.Prompts.Confirm(message, (sender, e) =>
                {

                }));
            }
        }

        /// <summary>
        /// Changes the content to a blank AssetEditor
        /// </summary>
        private void AddNewAsset()
        {
            //Todo FIx news
            _main.ContentFrame.Navigate(new AssetEditor(new AssetRepository(), new TagListController(new PrintHelper()), _main));
        }

        private void EditById(object parameter)
        {
            ulong id = 0;
            try
            {
                id = ulong.Parse(parameter.ToString());
            }
            finally
            {
                if (id == 0)
                    _main.AddNotification(new Notification("Error! Unknown ID"), 3500);
                else
                {
                    Asset asset = _listController.AssetList.Where(a => a.ID == id).First();
                    _main.ContentFrame.Navigate(new AssetEditor(new AssetRepository(), new TagListController(new PrintHelper()), _main, asset));
                }
            }
        }

        /// <summary>
        /// Changes the content to AssetEditor with the selected asset
        /// </summary>
        private void EditAsset(Asset asset)
        {
            if (asset != null)
                _main.ContentFrame.Navigate(new AssetEditor(
                    new AssetRepository(), 
                    new TagListController(new PrintHelper()),_main, asset));
            else
                _main.AddNotification(new Notification("Could not edit Asset", Notification.ERROR));
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
                _listController.Search(SearchQuery);
                Items = _listController.AssetList;
            }
            else
            {
                //AutoTag();
            }

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

                Console.WriteLine("In tag mode so go on!");
                
                if (TagSearchSuggestions != null && TagSearchSuggestions.Count > 0)
                {
                    ITagable tag = TagSearchSuggestions[0];

                    if (_tagHelper.IsParentSet())
                    {
                        Console.WriteLine("Think there is a parent!");
                        _tagHelper.AddToQuery(tag);
                        AppliedTags.Add(tag);
                        TagSearchProcess();
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
                            _tagHelper.AddToQuery(tag);
                            AppliedTags.Add(tag);
                            TagSearchProcess();
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
            try
            {
                _tagHelper = new TagHelper(new TagRepository(), new UserRepository(), new ObservableCollection<ITagable>());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

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
            AppliedTags.Clear();
        }

        /// <summary>
        /// Changes the content to ViewAsset with the selected asset
        /// </summary>
        private void ViewAsset()
        {
            // TODO: Redirect to viewAsset page
            if(IsSelectedAssetValid())
                _listController.ViewAsset(GetSelectedItem());
            else
                _main.AddNotification(new Notification("Could not view Asset", Notification.ERROR));
        }

        private void RemoveAssetByID(object parameter)
        {
            ulong id = 0;
            try
            {
                id = ulong.Parse(parameter.ToString());
            }
            finally
            {
                // Delete Asset and display notification
                _listController.Remove(GetSelectedItem());
                _main.AddNotification(new Notification("Asset " + GetSelectedItem().Name + " Was deleted", Notification.INFO));
            }
        }

        private void RemoveSelected()
        {
            if (IsSelectedAssetValid())
                RemoveAsset(GetSelectedItem());
            else
            {
                // Display error notification on error
                _main.AddNotification(new Notification("Could not delete Asset", Notification.ERROR));
            }
        }

        private void RemoveAsset(Asset asset)
        {
            // Prompt user for confirmation of removal
            _main.DisplayPrompt(new Views.Prompts.Confirm($"Are you sure you want to remove { asset.Name }?", (sender, e) =>
            {
                if (e.Result)
                {
                    _main.AddNotification(new Notification("Asset " + asset.Name + " was deleted", Notification.INFO));
                    _listController.Remove(asset);
                }
            }));
        }

        /// <summary>
        /// Exports selected assets to .csv file
        /// </summary>
        private void Export()
        {
            //TODO: Get selected assets
            // Look here for how to (Answer 2): https://stackoverflow.com/questions/2282138/wpf-listview-selecting-multiple-list-view-items
            List<Asset> selected = new List<Asset>();
            
            if(selected.Count < 1)
                _listController.Export(selected);
        }

        /// <summary>
        /// Returns the selected asset
        /// </summary>
        /// <returns>Selected Asset</returns>
        private Asset GetSelectedItem()
        {
            if (SelectedItemIndex == -1 || SelectedItemIndex >= _listController.AssetList.Count)
                return null;

            return _listController.AssetList[SelectedItemIndex];
        }

        /// <summary>
        /// Determines if the selected Asset is valid.
        /// </summary>
        /// <returns>Is selected Asset Valid</returns>
        private bool IsSelectedAssetValid()
        {
            Asset selectedAsset = _listController.AssetList[SelectedItemIndex];
            if (selectedAsset == null)
            {
                // Display error notification
                _main.AddNotification(new Notification("Selected Asset is not valid", Notification.ERROR));
                return false;
            }

            return true;
        }
        
    }
}
