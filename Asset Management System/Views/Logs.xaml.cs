using System.Windows.Input;
using System.Windows.Controls;
using Asset_Management_System.ViewModels;
using Asset_Management_System.Resources.DataModels;
using Asset_Management_System.Services.Interfaces;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for Logs.xaml
    /// </summary>
    public partial class Logs : Page
    {
        public Logs(MainViewModel main, IEntryService entryService)
        {
            InitializeComponent();
            DataContext = new ViewModels.LogsViewModel(main, entryService);
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LogsViewModel vm = this.DataContext as LogsViewModel;
            vm.ViewCommand.Execute(null);
        }
    }
}
