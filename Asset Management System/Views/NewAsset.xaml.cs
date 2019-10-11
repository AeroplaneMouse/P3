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
            Department department = Main.topNavigationPage.BtnShowDepartments.Content as Department;
            if (department != null)
                _asset.DepartmentID = department.ID;
            else
                Console.WriteLine("ERROR! Department not found.");

            // Creates a log entry, currently uses for testing.
            LogController logController = new LogController();
            Asset.Attach(logController);
            Asset.Notify();
            AssetRepository rep = new AssetRepository();
            rep.Insert(_asset);
            Main.ChangeSourceRequest(new Assets(Main));
        }
    }
}
