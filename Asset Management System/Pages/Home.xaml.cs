using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Events;
using System.Linq;
using System;

namespace Asset_Management_System.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public event NotificationEventHandler ShowNotification;

        public Home()
        {
            InitializeComponent();
        }

        private void Btn_Test_Click(object sender, RoutedEventArgs e)
        {
            ShowNotification(sender, "This is a test notification");
        }

        private void BtnShowDepartments_Click(object sender, RoutedEventArgs e)
        {
            DepartmentRepository dep = new DepartmentRepository();

            List<Department> objects = dep.GetAll();
            List<string> department_names = objects.Select(s => s.Name).ToList();
            LbDepartments.ItemsSource = department_names;
        }
    }
}
