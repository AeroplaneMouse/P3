using System.Windows.Controls;
using AMS.Controllers;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.ViewModels;

namespace AMS.Views
{
    public partial class LogList : Page
    {
        public LogList(MainViewModel main, ILogRepository logRepository, IExporter exporter)
        {
            InitializeComponent();
            DataContext = new LogListViewModel(main, new LogListController(logRepository, exporter));
        }
    }
}