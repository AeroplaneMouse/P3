﻿using System;
using System.Collections.ObjectModel;
using Asset_Management_System.Controllers;
using Asset_Management_System.Models;
using Asset_Management_System.Database.Repositories;
using System.Windows.Media;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for NewAsset.xaml
    /// </summary>
    public partial class NewAsset : FieldsController
    {
        private MainWindow Main;

        readonly Asset _asset = new Asset();
        public NewAsset(MainWindow main)
        {
            InitializeComponent();
            Main = main;
            FieldsControl.ItemsSource = FieldsList = new ObservableCollection<Field>();
        }

        private void BtnSaveNewAsset_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _asset.Name = TbName.Text;;
            _asset.Description = TbDescription.Text;
            foreach (var field in FieldsList)
            {
                _asset.AddField(field);
                Console.WriteLine(field.Content);
            }

            _asset.SerializeFields();
            Department department = Main.topNavigationPage.SelectedDepartment;
            if (department != null)
                _asset.DepartmentID = department.ID;
            else
            {
                string message = $"ERROR! No department set. Please create a department to attach the asset to.";
                Main.ShowNotification(sender, new Events.NotificationEventArgs(message, Brushes.Red));
                return;
            }

            // Creates a log entry, currently uses for testing.
            LogController logController = new LogController();
            _asset.Attach(logController);
            _asset.Notify();
            AssetRepository rep = new AssetRepository();
            rep.Insert(_asset);
            Main.ChangeSourceRequest(new Assets(Main));
        }
    }
}
