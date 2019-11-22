using System.Windows.Controls;
using AMS.Controllers;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories;
using AMS.Database.Repositories.Interfaces;
using AMS.Helpers;
using AMS.Interfaces;
using AMS.IO;
using AMS.ViewModels;

namespace AMS.Views
{
    /// <summary>
    /// Interaction logic for AssetListView.xaml
    /// </summary>
    public partial class AssetList : Page
    {
        public AssetList(MainViewModel main, IAssetRepository assetRepository, IExporter exporter, ICommentController commentController)
        {
            InitializeComponent();
            DataContext = new AssetListViewModel(main, new AssetListController(assetRepository, exporter), commentController);
        }
    }
}
