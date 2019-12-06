using System.Windows.Controls;
using Asset_Management_System.Models;
using Asset_Management_System.Services.Interfaces;
using Asset_Management_System.ViewModels;

namespace Asset_Management_System.Views
{
    public partial class AssetManager : Page
    {
        /// <summary>
        /// AssetManager is called when creating, or editing a asset.
        /// </summary>
        /// <param name="main"></param>
        /// <param name="inputAsset">Optional input, only used when editing a asset.</param>
        /// <param name="addMultiple"></param>
        public AssetManager(MainViewModel main, IAssetService service,  Asset inputAsset = null,bool addMultiple = false)
        {
            InitializeComponent();
            DataContext = new AssetManagerViewModel(main, inputAsset, service, InputBox, addMultiple);
        }
    }
}
