using AMS.Controllers.Interfaces;
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
    /// Interaction logic for UserListView.xaml
    /// </summary>
    public partial class UserList : Page
    {
        public UserList(IUserListController controller)
        {
            InitializeComponent();
            this.DataContext = new UserListViewModel(controller);
        }
        
        private GridViewColumnHeader _lastHeaderClicked;
        private ListSortDirection _lastDirectionSorted;
        
        // This code is based on: https://docs.microsoft.com/en-us/dotnet/framework/wpf/controls/how-to-sort-a-gridview-column-when-a-header-is-clicked?redirectedfrom=MSDN
        private void ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;

            GridViewColumnHeader header = e.OriginalSource as GridViewColumnHeader;
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
            ICollectionView dataView = CollectionViewSource.GetDefaultView(StartElement.ItemsSource);
 
            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
    }
}
