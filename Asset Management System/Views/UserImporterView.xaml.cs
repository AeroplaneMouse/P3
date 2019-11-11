using System.Windows;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for UserImporterView.xaml
    /// </summary>
    public partial class UserImporterView : Window
    {
        public UserImporterView()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.UserImporterViewModel();
        }
    }
}
