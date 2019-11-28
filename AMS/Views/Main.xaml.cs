using AMS.Database.Repositories.Interfaces;
using AMS.ViewModels;
using System.Windows;
using AMS.Helpers.Features;

namespace AMS.Views
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main(IUserRepository userRepository, IDepartmentRepository departmentRepository)
        {
            InitializeComponent();
            Features.Instance.Main = Features.Instance.Create.CreateMainViewModel(this);
            DataContext = Features.Instance.Main;
        }
    }
}
