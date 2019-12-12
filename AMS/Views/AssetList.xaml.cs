using AMS.ViewModels;
using AMS.Interfaces;
using AMS.Controllers;
using System.Windows.Controls;
using AMS.Database.Repositories.Interfaces;
using System.Collections;
using System.ComponentModel;
using System.DirectoryServices;
using AMS.Models;
using System.Windows;
using System.Windows.Data;
using System.Windows.Navigation;
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

        private GridViewColumnHeader _lastHeaderClicked;
        private ListSortDirection _lastDirectionSorted;

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AssetListViewModel viewModel = (DataContext as AssetListViewModel);
            viewModel.SelectedItems.Clear();

            foreach (Asset asset in (sender as ListView).SelectedItems)
                viewModel.SelectedItems.Add(asset);

            // Setting check all checkbox
            if (viewModel.SelectedItems.Count > 0)
                viewModel.CheckAll = true;
            else
                viewModel.CheckAll = false;

            // Only change visibility if current user if admin
            if (Features.GetCurrentSession().IsAdmin())
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

        // This code is based on: https://docs.microsoft.com/en-us/dotnet/framework/wpf/controls/how-to-sort-a-gridview-column-when-a-header-is-clicked
        private void ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;

            GridViewColumnHeader header = e.OriginalSource as GridViewColumnHeader;

            if (header?.Tag == null) return;
            
            string sortBy = header.Tag.ToString();
            ListSortDirection sortDirection;

            if (header != _lastHeaderClicked)
            {
                sortDirection = ListSortDirection.Ascending;
            }
            else
            {
                if (_lastDirectionSorted.Equals(ListSortDirection.Ascending))
                    sortDirection = ListSortDirection.Descending;
                else
                    sortDirection = ListSortDirection.Ascending;
            }

            _lastDirectionSorted = sortDirection;
            _lastHeaderClicked = header;
            Sort(sortBy, sortDirection);
        }
        
        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(ListOfAssets.ItemsSource);
 
            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
    }
}
