using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using AMS.Controllers.Interfaces;
using AMS.Models;

namespace AMS.ViewModels
{
    public class AssetListViewModel
    {
        private IAssetListController _listController;
        
        public List<Asset> Items { get; set; }
        public int SelectedItemIndex { get; set; }
        public string SearchQuery { get; set; }
        
        public ICommand AddNewCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ViewCommand { get; set; }

        public AssetListViewModel(IAssetListController listController)
        {
            _listController = listController;
            Items = _listController.AssetList;
            
            AddNewCommand = new Base.RelayCommand(AddNewAsset);
            EditCommand = new Base.RelayCommand(EditAsset);
            //PrintCommand = new Base.RelayCommand();
            SearchCommand = new Base.RelayCommand(SearchAssets);
            ViewCommand = new Base.RelayCommand(ViewAsset);
        }

        /// <summary>
        /// Handles the add new action
        /// </summary>
        private void AddNewAsset() => _listController.AddNew();

        /// <summary>
        /// Handles the edit asset action
        /// </summary>
        private void EditAsset() => _listController.Edit(GetSelectedItem());

        /// <summary>
        /// Searches the list for Assets matching the searchQuery
        /// </summary>
        private void SearchAssets() => _listController.Search(SearchQuery);

        /// <summary>
        /// Handles the view asset action
        /// </summary>
        private void ViewAsset() => _listController.ViewAsset(GetSelectedItem());

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
