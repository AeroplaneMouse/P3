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
        private MainWindow Main;

        public Home(MainWindow main)
        {
            InitializeComponent();
            Main = main;
        }

        private void Btn_Test_Click(object sender, RoutedEventArgs e)
        {
            NotificationEventArgs args = new NotificationEventArgs("Test notification", Brushes.Green);
            Main.ShowNotification(this, args);
            //ShowNotification(sender, args);
        }

        private void BtnShowDepartments_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
