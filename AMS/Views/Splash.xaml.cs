using AMS.ViewModels;
using System.Windows.Controls;
using AMS.Database.Repositories.Interfaces;

namespace AMS.Views
{
    /// <summary>
    /// Interaction logic for SplashPage.xaml
    /// </summary>
    public partial class Splash : Page
    {
        public Splash(MainViewModel main, IUserRepository userRepository)
        {
            InitializeComponent();
            DataContext = new SplashViewModel(main, userRepository);
        }
    }
}
