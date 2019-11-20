using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AMS.Views;
using AMS.Controllers;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories;
using AMS.Helpers;
using AMS.Models;
using AMS.ViewModels.Base;
using System.Linq;

namespace AMS.ViewModels
{
    public class AssetListViewModel : BaseViewModel
    {
        private IAssetListController _listController;
        public MainViewModel Main { get; set; }
        
        public ObservableCollection<Asset> Items { get; set; }
        public int SelectedItemIndex { get; set; }
        public string SearchQuery { get; set; }
        
        public ICommand AddNewCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand EditByIdCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ViewCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand RemoveByIdCommand { get; set; }

        public AssetListViewModel(MainViewModel main, IAssetListController listController)
        {
            Main = main;
            _listController = listController;
            Items = _listController.AssetList;
            
            AddNewCommand = new RelayCommand(AddNewAsset);
            EditCommand = new RelayCommand(EditAsset);
            PrintCommand = new RelayCommand(Export);
            SearchCommand = new RelayCommand(SearchAssets);
            ViewCommand = new RelayCommand(ViewAsset);
            RemoveCommand = new RelayCommand(RemoveSelected);
            EditByIdCommand = new RelayCommand<object>(EditById);
            RemoveByIdCommand = new RelayCommand<object>(RemoveAssetByID);
        }

        /// <summary>
        /// Changes the content to a blank AssetEditor
        /// </summary>
        private void AddNewAsset()
        {
            //Todo FIx news
            Main.ContentFrame.Navigate(new AssetEditor(new AssetRepository(), new TagListController(new PrintHelper()), Main));
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
                    Main.AddNotification(new Notification("Error! Unknown ID"), 3500);
                else
                {
                    Asset asset = _listController.AssetList.Where(a => a.ID == id).First();
                    Main.ContentFrame.Navigate(new AssetEditor(new AssetRepository(), new TagListController(new PrintHelper()), Main, asset));
                }
            }
        }

        /// <summary>
        /// Changes the content to AssetEditor with the selected asset
        /// </summary>
        private void EditAsset()
        {

            //Todo FIx news
            
            if (IsSelectedAssetValid())
                Main.ContentFrame.Navigate(new AssetEditor(new AssetRepository(), new TagListController(new PrintHelper()),Main, GetSelectedItem()));
            else
                Main.AddNotification(new Notification("Could not edit Asset", Notification.ERROR));
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
                Main.AddNotification(new Notification("Could not view Asset", Notification.ERROR));
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
                Main.AddNotification(new Notification("Asset " + GetSelectedItem().Name + " Was deleted", Notification.INFO));
            }
        }

        private void RemoveSelected()
        {
            if (IsSelectedAssetValid())
                RemoveAsset(GetSelectedItem());
            else
            {
                // Display error notification on error
                Main.AddNotification(new Notification("Could not delete Asset", Notification.ERROR));
            }
        }

        private void RemoveAsset(Asset asset)
        {
            // Prompt user for confirmation of removal
            Main.DisplayPrompt(new Views.Prompts.Confirm($"Are you sure you want to remove { asset.Name }?", (sender, e) =>
            {
                if (e.Result)
                {
                    Main.AddNotification(new Notification("Asset " + asset.Name + " was deleted", Notification.INFO));
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
                Main.AddNotification(new Notification("Selected Asset is not valid", Notification.ERROR));
                return false;
            }

            return true;
        }
        
    }
}
