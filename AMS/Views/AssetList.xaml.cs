using System.Windows.Controls;
using AMS.Controllers;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories;
using AMS.Helpers;
using AMS.IO;
using AMS.ViewModels;

namespace AMS.Views
{
    /// <summary>
    /// Interaction logic for AssetListView.xaml
    /// </summary>
    public partial class AssetList : Page
    {
        public AssetList(MainViewModel main, IAssetListController assetListController)
        {
            InitializeComponent();
            DataContext = new ViewModels.AssetListViewModel(main, assetListController);
        }
    }
}
