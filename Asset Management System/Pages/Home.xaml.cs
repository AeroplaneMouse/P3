using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Events;
using System.Linq;

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
            NotificationEventArgs args = new NotificationEventArgs("Test notification", Brushes.Green);
            ShowNotification(sender, args);
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
