using System;
using System.Collections.ObjectModel;
using Asset_Management_System.Controllers;
using Asset_Management_System.Models;
using Asset_Management_System.Database.Repositories;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using Asset_Management_System.ViewModels;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for NewAsset.xaml
    /// </summary>
    public partial class NewAsset : FieldsController
    {
        private MainViewModel _main;

        readonly Asset _asset = new Asset();

        public NewAsset(MainViewModel main)
        {
            InitializeComponent();
            _main = main;
            FieldsControl.ItemsSource = FieldsList = new ObservableCollection<Field>();
        }

        private void BtnSaveNewAsset_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //_asset.Name = TbName.Text;
            //;
            //_asset.Description = TbDescription.Text;
            //foreach (var field in FieldsList)
            //{
            //    _asset.AddField(field);
            //    Console.WriteLine(field.Content);
            //}

            //_asset.SerializeFields();
            //Department department = _main.topNavigationPage.SelectedDepartment;
            //if (department != null)
            //{
            //    _asset.DepartmentID = department.ID;
            //    // Creates a log entry, currently uses for testing.
            //    LogController logController = new LogController();
            //    _asset.Attach(logController);
            //    _asset.Notify();
            //    AssetRepository rep = new AssetRepository();
            //    rep.Insert(_asset);
            //    //Main.ChangeSourceRequest(new Assets(Main));
            //}
            //else
            //{
            //    string message = $"ERROR! No department set. Please create a department to attach the asset to.";
            //    Main.ShowNotification(sender, new Events.NotificationEventArgs(message, Brushes.Red));
            //}

            //Console.WriteLine("List of the current fields after adding the field:");
            //Console.WriteLine("---------------------------------------");
            //Console.WriteLine("ID  |   Field name   |   Content of the field");
            //foreach (var test in FieldsList)
            //{

            //    Console.WriteLine(test.ID + " | " + test.Label + " | " + test.Content);
                


            //}

            //Console.WriteLine("---------------------------------------");
        }

        /// <summary>
        /// Function to remove a field from the list of fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotSupportedException"></exception>
        private void OnDeleteField(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button)?.Name)
            {
                case "DeleteTextField":
                    Console.WriteLine("Textfield removed");
                    break;
                case "DeleteStringField":
                    Console.WriteLine("StringField removed");
                    break;
                case "DeleteIntegerField":
                    Console.WriteLine("IntegerField removed");
                    break;
                case "DeleteDateField":
                    Console.WriteLine("DataField removed");
                    break;
                case "DeleteBooleanField":
                    Console.WriteLine("BooleanField removed");
                    break;
                default:
                    throw new NotSupportedException();
            }

            FieldsList.Remove((sender as FrameworkElement).DataContext as Field);
            Console.WriteLine("List of the current fields after removing the field:");
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("ID |Field name | Content of the field");
            foreach (var test in FieldsList)
            {
                Console.WriteLine(test.ID + " | " + test.Label + "|" + test.Content);
                
            }

            Console.WriteLine("---------------------------------------");
        }
    }
}