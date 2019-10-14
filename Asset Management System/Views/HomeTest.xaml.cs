using System.Windows.Controls;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for HomeTest.xaml
    /// </summary>
    public partial class HomeTest : Page
    {
        public HomeTest()
        {
            InitializeComponent();
            DataContext = new ViewModels.HomeViewModel();
        }
    }
}
