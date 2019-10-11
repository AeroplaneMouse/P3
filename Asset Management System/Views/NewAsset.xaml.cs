using System;
using System.Collections.ObjectModel;
using Asset_Management_System.Controllers;
using Asset_Management_System.Models;
using Asset_Management_System.Database.Repositories;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for NewAsset.xaml
    /// </summary>
    public partial class NewAsset : FieldsController
    {
        private MainWindow Main;

        public NewAsset(MainWindow main)
        {
            InitializeComponent();
            Asset = new Asset();
            Main = main;
            FieldsControl.ItemsSource = FieldsList = new ObservableCollection<Field>();
        }

        private void BtnSaveNewAsset_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string name = TbName.Text;
            string description = TbDescription.Text;
            Asset.Name = name;
            Asset.Description = description;
            foreach (var field in FieldsList)
            {
                Asset.AddField(field);
                Console.WriteLine(field.Content);
            }

            Asset.SerializeFields();
            Department department = Main.topNavigationPage.BtnShowDepartments.Content as Department;
            if (department != null)
                Asset.DepartmentID = department.ID;
            else
                Console.WriteLine("ERROR! Department not found.");

            // Creates a log entry, currently uses for testing.
            LogController logController = new LogController();
            Asset.Attach(logController);
            Asset.Notify();
            AssetRepository rep = new AssetRepository();
            rep.Insert(Asset);
            Main.ChangeSourceRequest(new Assets(Main));
        }
    }
}
