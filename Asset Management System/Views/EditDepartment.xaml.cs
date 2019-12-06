using System.Windows.Controls;
using Asset_Management_System.Models;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for EditDepartment.xaml
    /// </summary>
    public partial class EditDepartment : Page
    {
        public EditDepartment(Department department)
        {
            InitializeComponent();
            Test.Content = $"You are now on a new page, where you can edit the department: { department.ID } : { department.Name}";
        }
    }
}
