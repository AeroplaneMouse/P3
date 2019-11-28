using AMS.Logging;
using AMS.ViewModels;
using System.Windows.Controls;

namespace AMS.Views
{
    /// <summary>
    /// Interaction logic for LogPresenter.xaml
    /// </summary>
    public partial class LogPresenter : Page
    {
        public LogPresenter(LogEntry entry)
        {
            InitializeComponent();
            DataContext = new LogPresenterViewModel(entry);
        }
    }
}
