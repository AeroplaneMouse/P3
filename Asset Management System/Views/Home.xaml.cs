using System.Windows.Controls;
using Asset_Management_System.ViewModels;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public Home(MainViewModel main)
        {
            InitializeComponent();
            DataContext = new ViewModels.HomeViewModel(main);
        }
    }
}
