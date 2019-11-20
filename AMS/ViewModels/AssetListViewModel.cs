﻿using AMS.Views;
using AMS.Models;
using AMS.ViewModels.Base;
using System.Windows.Input;
using System.Collections.Generic;
using AMS.Controllers.Interfaces;
using System.Collections.ObjectModel;

namespace AMS.ViewModels
{
    public class AssetListViewModel : BaseViewModel
    {
        private IAssetListController _listController;
        private MainViewModel _main;
        
        public ObservableCollection<Asset> Items { get; set; }
        public int SelectedItemIndex { get; set; }
        public string SearchQuery { get; set; }
        
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
            if (SearchQuery == null) return;
            _listController.Search(SearchQuery);
            Items = _listController.AssetList;
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
                // Prompt user for confirmation of removal
                _main.DisplayPrompt(new Views.Prompts.Confirm($"Are you sure you want to remove { GetSelectedItem().Name }?", (sender, e) =>
                {
                    if (e.Result)
                    {
                        _main.AddNotification(new Notification("Asset " + GetSelectedItem().Name + " was deleted", Notification.INFO));
                        _listController.Remove(GetSelectedItem());
                    }
                }));
            }
            else
            {
                // Display error notification on error
                _main.AddNotification(new Notification("Could not delete asset", Notification.ERROR));
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
            return GetSelectedItem() != null;
        }
        
    }
}
