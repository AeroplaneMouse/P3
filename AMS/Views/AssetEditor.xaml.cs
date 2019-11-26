using AMS.Models;
using AMS.ViewModels;
using AMS.Controllers;
using System.Windows.Controls;
using AMS.Database.Repositories.Interfaces;
using AMS.Controllers.Interfaces;
using AMS.Helpers;

namespace AMS.Views
{
    public partial class AssetEditor : Page
    {
        public AssetEditor(IAssetController assetController, ITagListController tagListController, TagHelper tagHelper)
        {
            InitializeComponent();
            DataContext = new AssetEditorViewModel(assetController, tagListController, tagHelper);
        }
    }
}