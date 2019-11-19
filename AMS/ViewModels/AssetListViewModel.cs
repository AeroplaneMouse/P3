using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Text;
using System.Windows.Input;
using AMS.Controllers.Interfaces;
using AMS.Models;
using AMS.ViewModels.Base;

namespace AMS.ViewModels
{
    public class AssetListViewModel : BaseViewModel
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
        public ICommand RemoveCommand { get; set; }

        public AssetListViewModel(IAssetListController listController)
        {
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
