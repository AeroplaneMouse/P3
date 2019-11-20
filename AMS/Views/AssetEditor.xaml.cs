using System.Windows.Controls;
using AMS.Controllers;
using AMS.Database.Repositories;
using AMS.Database.Repositories.Interfaces;
using AMS.Models;
using AMS.ViewModels;

namespace AMS.Views
{
    public partial class AssetEditor : Page
    {
        public AssetEditor(IAssetRepository assetRepository,TagListController tagListController,MainViewModel main,Asset asset = null)
        {
            InitializeComponent();
            DataContext = new ViewModels.AssetEditorViewModel(asset, assetRepository, tagListController,main);
        }
    }
}