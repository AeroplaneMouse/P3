using AMS.Database.Repositories.Interfaces;
using AMS.ViewModels;
using System.Windows;

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
            DataContext = new MainViewModel(this, userRepository, departmentRepository);
        }
    }
}
