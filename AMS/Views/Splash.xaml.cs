using AMS.ViewModels;
using System.Windows.Controls;
using AMS.Services.Interfaces;

namespace AMS.Views
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
