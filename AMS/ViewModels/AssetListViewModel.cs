using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Text;
using System.Windows.Input;
using AMS.Controllers.Interfaces;
using AMS.Models;
using AMS.ViewModels.Base;
using AMS.Views;

namespace AMS.ViewModels
{
    public class AssetListViewModel : BaseViewModel
    {
        private IAssetListController _listController;
        private MainViewModel _main;
        
        public List<Asset> Items { get; set; }
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
            
            AddNewCommand = new Base.RelayCommand(AddNewAsset);
            EditCommand = new Base.RelayCommand(EditAsset);
            PrintCommand = new Base.RelayCommand(Export);
            SearchCommand = new Base.RelayCommand(SearchAssets);
            ViewCommand = new Base.RelayCommand(ViewAsset);
            RemoveCommand = new Base.RelayCommand(RemoveAsset);
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
            _main.ContentFrame.Navigate(new AssetEditor(GetSelectedItem()));
        }

        /// <summary>
        /// Searches the list for Assets matching the searchQuery
        /// </summary>
        private void SearchAssets() => _listController.Search(SearchQuery);

        /// <summary>
        /// Changes the content to ViewAsset with the selected asset
        /// </summary>
        private void ViewAsset()
        {
            // TODO: Redirect to viewAsset page
            _listController.ViewAsset(GetSelectedItem());
        }

        /// <summary>
        /// Deletes the selected asset
        /// </summary>
        private void RemoveAsset()
        {
            _listController.Remove(GetSelectedItem());
            
            //TODO: Display notification?
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
        /// Returns the selected asset, or throws an error if it is not valid
        /// </summary>
        /// <returns>Selected Asset</returns>
        private Asset GetSelectedItem()
        {
            Asset selectedAsset = _listController.AssetList[SelectedItemIndex];
            //TODO: Handle error
            if(selectedAsset == null)
                throw new NullReferenceException("The selected asset is not valid");

            return selectedAsset;
        }
        
    }
}
