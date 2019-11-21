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
        private MainViewModel _main;
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
        public ICommand RemoveBySelectionCommand { get; set; }
        public ICommand EditBySelectionCommand { get; set; }
        public int SelectedItemIndex { get; private set; }

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
            RemoveBySelectionCommand = new RelayCommand(RemoveSelected);
            EditBySelectionCommand = new RelayCommand(EditBySelection);
            AutoTagCommand = new RelayCommand(AutoTag);
            ClearInputCommand = new RelayCommand(ClearInput);
        }

        private void EditBySelection()
        {
            // Only ONE item can be edited
            if (SelectedItems.Count == 1)
            {
                throw new NotImplementedException();
            }
            else
                _main.AddNotification(new Notification("Error! Please select one and only one asset.", Notification.ERROR), 3500);
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
            //AppliedTags.Clear();
        }

        /// <summary>
        /// Changes the content to ViewAsset with the selected asset
        /// </summary>
        private void ViewAsset()
        {
            // TODO: Redirect to viewAsset page
            if (SelectedItems.Count == 1)
                _listController.ViewAsset(SelectedItems.First());
            else
                _main.AddNotification(new Notification("Error! Could not view asset", Notification.ERROR));
        }

        /// <summary>
        /// Removes the given asset and displays a prompt
        /// </summary>
        /// <param name="asset"></param>
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
        /// Removes all the selected items
        /// </summary>
        private void RemoveSelected()
        {
            if (SelectedItems.Count > 0)
            {
                // Prompt user for approval for the removal of x assets 
                string message = $"Are you sure you want to remove { SelectedItems.Count } asset";
                message += SelectedItems.Count > 1 ? "s?" : "?";
                _main.DisplayPrompt(new Views.Prompts.Confirm(message, (sender, e) =>
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

                        _main.AddNotification(
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
            //TODO: Get selected assets
            // Look here for how to (Answer 2): https://stackoverflow.com/questions/2282138/wpf-listview-selecting-multiple-list-view-items
            // Fly: Jeg tror, at jeg har lavet det.
            
            if (SelectedItems.Count > 0)
                // Export selected items
                _listController.Export(SelectedItems);
            else
                // Export all items found by search
                _listController.Export(ObservableToList(Items));
        }

        // This is super stupid. Please fix
        // Takes an ObservableCollecion and returns a List with the items
        // of the observableCollection
        private List<Asset> ObservableToList(ObservableCollection<Asset> list)
        {
            List<Asset> newList = new List<Asset>();
            foreach (Asset asset in list)
                newList.Add(asset);
            return newList;
        }
    }
}
