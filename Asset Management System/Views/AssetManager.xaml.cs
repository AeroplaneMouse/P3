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
    public partial class AssetManager : FieldsController
    {
        private MainWindow Main;
        private Asset _asset;
        private bool Editing;

        /// <summary>
        /// AssetManager is called when creating, or editing a asset.
        /// </summary>
        /// <param name="main"></param>
        /// <param name="inputTag">Optional input, only used when editing a asset.</param>
        public AssetManager(MainWindow main, Asset inputAsset = null)
        {
            InitializeComponent();
            Main = main;
            FieldsList = new ObservableCollection<Field>();
            FieldsControl.ItemsSource = FieldsList = new ObservableCollection<Field>();
            if (inputAsset != null)
            {
                _asset = inputAsset;
                LoadFields();
                Editing = true;
            }
            else
            {
                _asset = new Asset();
                Editing = false;
            }

        }

        /// <summary>
        /// This function fires when the "Save Asset" button is clicked.
        /// The function saves or updates the asset in the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSaveNewAsset_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _asset.Name = TbName.Text;
            _asset.Description = TbDescription.Text;
            foreach (var field in FieldsList)
            {
                _asset.AddField(field);
            }

            _asset.SerializeFields();
            Department department = Main.topNavigationPage.SelectedDepartment;
            if (department != null)
            {
                _asset.DepartmentID = department.ID;
                // Creates a log entry, currently uses for testing.
                LogController logController = new LogController();
                _asset.Attach(logController);
                _asset.Notify();
                AssetRepository rep = new AssetRepository();
                if (Editing)
                {
                    rep.Update(_asset);
                }
                else
                {
                    rep.Insert(_asset);
                }

                Main.ChangeSourceRequest(new Assets(Main));
            }
            else
            {
                string message = $"ERROR! No department set. Please create a department to attach the asset to.";
                Main.ShowNotification(sender, new Events.NotificationEventArgs(message, Brushes.Red));
            }
        }
        
        /// <summary>
        /// Runs through the saved fields within the tag, and adds these to the fieldList.
        /// </summary>
        /// <returns></returns>
        private bool LoadFields()
        {
            ConsoleWriter.ConsoleWrite("------Field labels | Field content -------");
            _asset.DeserializeFields();
            foreach (var field in _asset.FieldsList)
            {
                ConsoleWriter.ConsoleWrite(field.Label +" | "+ field.Content);
                FieldsList.Add(field);
            }
            TbName.Text = _asset.Name ;
            TbDescription.Text = _asset.Description;

            return true;
        }
    }
}