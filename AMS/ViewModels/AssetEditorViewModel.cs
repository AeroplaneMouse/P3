using System;
using System.Collections.Generic;
using System.Windows.Input;
using AMS.Controllers;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.Models;
using AMS.ViewModels.Base;

namespace AMS.ViewModels
{
    public class AssetEditorViewModel : BaseViewModel
    {
        private MainViewModel _main;
        private AssetController _assetController;
        private TagListController _tagListController;

        public ICommand SaveAssetCommand;


        public List<ITagable> CurrentlyAddedTags => _assetController.CurrentlyAddedTags;

        public string Name
        {
            get => _assetController.Asset.Name;
            set => _assetController.Asset.Name = value;
        }

        public string Identifier
        {
            get => _assetController.Asset.Identifier;
            set => _assetController.Asset.Identifier = value;
        }

        public string Description
        {
            get => _assetController.Asset.Identifier;
            set => _assetController.Asset.Description = value;
        }

        public string Title { get; set; }

        public AssetEditorViewModel(Asset asset, IAssetRepository assetRepository, TagListController tagListController,
            MainViewModel main)
        {
            _assetController = new AssetController(asset, assetRepository);
            _tagListController = tagListController;
            Title = "Edit asset";

            //Commands
            SaveAssetCommand = new RelayCommand(() => _assetController.Save());
        }
    }
}