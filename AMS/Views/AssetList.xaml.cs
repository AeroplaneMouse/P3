using AMS.ViewModels;
using AMS.Interfaces;
using AMS.Controllers;
using System.Windows.Controls;
using AMS.Database.Repositories.Interfaces;
using System.Collections;
using AMS.Models;
using System.Windows;
using AMS.Controllers.Interfaces;
using AMS.Helpers;

namespace AMS.Views
{
    /// <summary>
    /// Interaction logic for AssetListView.xaml
    /// </summary>
    public partial class AssetList : Page
    {
        public AssetList(IAssetListController controller, TagHelper tagHelper)
        {
            InitializeComponent();
            DataContext = new AssetListViewModel(controller, tagHelper);
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AssetListViewModel viewModel = (DataContext as AssetListViewModel);
            viewModel.SelectedItems.Clear();

            foreach (Asset asset in (sender as ListView).SelectedItems)
                viewModel.SelectedItems.Add(asset);

            // Only change visibility if current user if admin
            if (Features.Main.CurrentSession.IsAdmin())
            {
                // Setting single item selected visibility
                viewModel.SingleSelected = viewModel.SelectedItems.Count == 1 ?
                    Visibility.Visible :
                    Visibility.Collapsed;

                // Setting multiple item selected visibility
                viewModel.MultipleSelected = viewModel.SelectedItems.Count > 0 ?
                        Visibility.Visible :
                        Visibility.Collapsed;
            }
        }
    }
}
