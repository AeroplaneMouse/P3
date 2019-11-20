using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories;
using AMS.Helpers;
using AMS.Models;
using AMS.ViewModels.Base;
using AMS.Views;

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

        public ICommand AddNewCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ViewCommand { get; set; }
        public ICommand RemoveCommand { get; set; }

        public AssetListViewModel(MainViewModel main, IAssetListController listController)
        {
            _main = main;
            _listController = listController;
            Items = _listController.AssetList;
            
            AddNewCommand = new RelayCommand(AddNewAsset);
            EditCommand = new RelayCommand(EditAsset);
            PrintCommand = new RelayCommand(Export);
            SearchCommand = new RelayCommand(SearchAssets);
            ViewCommand = new RelayCommand(ViewAsset);
            RemoveCommand = new RelayCommand(RemoveAsset);
        }

        /// <summary>
        /// Changes the content to a blank AssetEditor
        /// </summary>
        private void AddNewAsset()
        {
            // TODO: Change so it is not necessary to create new page here
            _main.ContentFrame.Navigate(new AssetEditor());
        }

        /// <summary>
        /// Changes the content to AssetEditor with the selected asset
        /// </summary>
        private void EditAsset()
        {
            if (IsSelectedAssetValid())
                _main.ContentFrame.Navigate(new AssetEditor(GetSelectedItem()));
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

            /*
            if (SearchQuery == null) return;
            _listController.Search(SearchQuery);
            Items = _listController.AssetList;
            */
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
                    inTagMode = false;
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

                        foreach (var item in TagSearchSuggestions)
                        {
                            Console.WriteLine(item.TagLabel);
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

            
            //List<ITagable> list = _tagHelper.Suggest(_searchQuery);
            //TagSearchSuggestions = new ObservableCollection<ITagable>(_tagHelper.SuggestedTags);
            
           
            /*
            foreach (var item in list)
            {
                Console.WriteLine(item.TagLabel);
            }

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
            */
        }

        private void LeavingTagMode()
        {
            
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
            // TODO: Redirect to viewAsset page
            if(IsSelectedAssetValid())
                _listController.ViewAsset(GetSelectedItem());
            else
                _main.AddNotification(new Notification("Could not view Asset", Notification.ERROR));
        }

        /// <summary>
        /// Deletes the selected asset
        /// </summary>
        private void RemoveAsset()
        {
            if (IsSelectedAssetValid())
            {
                // Delete Asset and display notification
                _listController.Remove(GetSelectedItem());
                _main.AddNotification(new Notification("Asset " + GetSelectedItem().Name + " Was deleted", Notification.INFO));
            }
            else
            {
                // Display error notification on error
                _main.AddNotification(new Notification("Could not delete Asset", Notification.ERROR));
            }
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
            return _listController.AssetList[SelectedItemIndex];
        }

        /// <summary>
        /// Determines if the selected Asset is valid.
        /// Displays error notification if Asset is not value
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
