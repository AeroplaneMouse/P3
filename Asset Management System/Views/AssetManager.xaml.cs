using System.Windows.Controls;
using Asset_Management_System.Models;
using Asset_Management_System.ViewModels;

namespace Asset_Management_System.Views
{
    public partial class AssetManager : Page
    {
        /// <summary>
        /// AssetManager is called when creating, or editing a asset.
        /// </summary>
        /// <param name="main"></param>
        /// <param name="inputTag">Optional input, only used when editing a asset.</param>
        public AssetManager(MainViewModel main, Asset inputAsset = null)
        {
            InitializeComponent();
            DataContext = new AssetManagerViewModel(main, inputAsset, InputBox);
        }
    }
}