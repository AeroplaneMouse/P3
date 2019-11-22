using AMS.Database.Repositories.Interfaces;
using System.Windows.Controls;

namespace AMS.Views
{
    public partial class Home : Page
    {
        public Home(IUserRepository userRepository, IAssetRepository assetRepository, ITagRepository tagRepository, IDepartmentRepository departmentRepository)
        {
            InitializeComponent();
            DataContext = new ViewModels.HomeViewModel(userRepository, assetRepository, tagRepository, departmentRepository);
        }
    }
}