using AMS.Models;
using AMS.ViewModels;
using AMS.Controllers;
using System.Windows.Controls;
using AMS.Database.Repositories.Interfaces;

namespace AMS.Views
{
    public partial class AssetEditor : Page
    {
        public AssetEditor(IAssetRepository assetRepository, TagListController tagListController, MainViewModel main, Asset asset = null)
        {
            InitializeComponent();
            DataContext = new ViewModels.AssetEditorViewModel(asset, assetRepository, tagListController, main);
        }
    }
}