using System.Windows.Controls;
using AMS.Controllers;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.Logging;
using AMS.ViewModels;
using System.Windows;


namespace AMS.Views
{
    public partial class LogList : Page
    {
        public LogList(MainViewModel main, ILogRepository logRepository, IExporter exporter)
        {
            InitializeComponent();
            DataContext = new LogListViewModel(main, new LogListController(logRepository, exporter));
        }
        
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LogListViewModel viewModel = (DataContext as LogListViewModel);
            viewModel.SelectedItems.Clear();

            foreach (Entry entry in (sender as ListView).SelectedItems)
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
    }
}