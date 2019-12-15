using AMS.Controllers.Interfaces;
using AMS.Interfaces;
using AMS.Logging;
using AMS.Models;
using AMS.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AMS.Views
{
    /// <summary>
    /// Interaction logic for AssetPresenter.xaml
    /// </summary>
    public partial class AssetPresenter : Page
    {
        public AssetPresenter(List<ITagable> tagList, IAssetController assetController, ICommentListController commentListController, ILogListController logListController, int tabIndex = 0)
        {
            InitializeComponent();
            DataContext = new AssetPresenterViewModel(assetController, commentListController, logListController, tabIndex);
        }

        private GridViewColumnHeader _lastHeaderClicked;
        private ListSortDirection _lastDirectionSorted;

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LogListViewModel viewModel = (DataContext as AssetPresenterViewModel).Tabs[2] as LogListViewModel;
            viewModel.SelectedItems.Clear();

            foreach (LogEntry entry in (sender as ListView).SelectedItems)
                viewModel.SelectedItems.Add(entry);

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

        // This code is based on: https://docs.microsoft.com/en-us/dotnet/framework/wpf/controls/how-to-sort-a-gridview-column-when-a-header-is-clicked?redirectedfrom=MSDN
        //private void ColumnHeader_Click(object sender, RoutedEventArgs e)
        //{
        //    if (sender == null) return;

        //    GridViewColumnHeader header = e.OriginalSource as GridViewColumnHeader;
        //    string sortBy = header?.Tag.ToString();
        //    ListSortDirection sortDirection;

        //    if (header != _lastHeaderClicked)
        //    {
        //        sortDirection = ListSortDirection.Ascending;
        //    }
        //    else
        //    {
        //        if (_lastDirectionSorted.Equals(ListSortDirection.Ascending))
        //            sortDirection = ListSortDirection.Descending;
        //        else
        //            sortDirection = ListSortDirection.Ascending;
        //    }

        //    _lastDirectionSorted = sortDirection;
        //    _lastHeaderClicked = header;
        //    Sort(sortBy, sortDirection);
        //}

        //private void Sort(string sortBy, ListSortDirection direction)
        //{
        //    ICollectionView dataView = CollectionViewSource.GetDefaultView(listViewWithItems.ItemsSource);

        //    dataView.SortDescriptions.Clear();
        //    SortDescription sd = new SortDescription(sortBy, direction);
        //    dataView.SortDescriptions.Add(sd);
        //    dataView.Refresh();
        //}
    }
}
