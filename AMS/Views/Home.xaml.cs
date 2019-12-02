using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using System.Windows.Controls;

namespace AMS.Views
{
    public partial class Home : Page
    {
        public Home(IHomeController homeController)
        {
            InitializeComponent();
            DataContext = new ViewModels.HomeViewModel(homeController);
        }
    }
}