using System.Windows.Controls;
using Asset_Management_System.ViewModels;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for SplashPage.xaml
    /// </summary>
    public partial class Splash : Page
    {
        public Splash(MainViewModel main)
        {
            InitializeComponent();
            DataContext = new ViewModels.SplashViewModel(main);
        }
    }
}
