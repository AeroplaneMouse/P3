using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Asset_Management_System.Models;

namespace Asset_Management_System.Pages
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
