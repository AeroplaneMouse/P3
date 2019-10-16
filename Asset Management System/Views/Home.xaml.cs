using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Events;
using System.Linq;

namespace Asset_Management_System.Views
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
            this.DataContext = new ViewModels.HomeViewModelTest();
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

        // TODO: Cursor stuff
        private void InputBox_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            //var txtBx = sender as TextBox;

            //if (txtBx == null || txtBx.Text == null)
            //{
            //    return;
            //}

            //txtBx.CaretIndex = txtBx.Text.Length + 1;

            //if (txtBx.CaretIndex == 2 || txtBx.CaretIndex == 5)
            //{
            //    txtBx.CaretIndex = txtBx.CaretIndex + 1;
            //}
        }
    }
}
