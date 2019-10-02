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

namespace Asset_Management_System.Pages
{
    /// <summary>
    /// Interaction logic for EditDepartment.xaml
    /// </summary>
    public partial class EditDepartment : Page
    {
        private TopNavigationPart2 nav;
        public EditDepartment(TopNavigationPart2 nav)
        {
            InitializeComponent();
            this.nav = nav;

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(GetSelectedDepartment));
        }

        private void GetSelectedDepartment()
        {
            string department = nav.LbDepartments.SelectedItem.ToString();
            Test.Content = department;
        }
    }
}
