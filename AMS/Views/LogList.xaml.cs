using System.ComponentModel;
using System.DirectoryServices;
using System.Windows.Controls;
using AMS.Controllers;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.Logging;
using AMS.ViewModels;
using System.Windows;
using System.Windows.Data;
using AMS.Controllers.Interfaces;

namespace AMS.Views
{
    public partial class LogList : Page
    {
        public LogList(ILogListController logListController)
        {
            InitializeComponent();
            DataContext = new LogListViewModel(logListController);
        }
        
        private GridViewColumnHeader _lastHeaderClicked;
        private ListSortDirection _lastDirectionSorted;
        
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LogListViewModel viewModel = (DataContext as LogListViewModel);
            viewModel.SelectedItems.Clear();

            foreach (LogEntry entry in (sender as ListView).SelectedItems)
                viewModel.SelectedItems.Add(entry);

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
        
        // This code is based on: https://docs.microsoft.com/en-us/dotnet/framework/wpf/controls/how-to-sort-a-gridview-column-when-a-header-is-clicked?redirectedfrom=MSDN
        private void ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;

            GridViewColumnHeader header = e.OriginalSource as GridViewColumnHeader;
            
            if (header?.Tag == null) return;
            
            string sortBy = header?.Tag.ToString();
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
            ICollectionView dataView = CollectionViewSource.GetDefaultView(listViewWithItems.ItemsSource);
 
            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
    }
}