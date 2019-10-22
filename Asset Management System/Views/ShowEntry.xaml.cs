using System.Windows;
using Asset_Management_System.Logging;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for DialogBoxWithInput.xaml
    /// </summary>
    public partial class ShowEntry : Window
    {
        public ShowEntry(Entry entry)
        {
            InitializeComponent();
            DataContext = new ViewModels.EntryViewModel(this, entry);
        }
    }
}