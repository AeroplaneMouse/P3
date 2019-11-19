using System.Windows.Controls;
using AMS.Controllers;
using AMS.Database.Repositories;
using AMS.IO;

namespace AMS.Views
{
    /// <summary>
    /// Interaction logic for AssetListView.xaml
    /// </summary>
    public partial class AssetList : Page
    {
        public AssetList()
        {
            InitializeComponent();
            DataContext = new ViewModels.AssetListViewModel(new AssetListController(new AssetRepository(), new Exporter()));
        }
    }
}
