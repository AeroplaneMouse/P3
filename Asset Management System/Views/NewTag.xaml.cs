using System;
using System.Collections.ObjectModel;
using Asset_Management_System.Models;
using Asset_Management_System.Database.Repositories;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for NewTag.xaml
    /// </summary>
    public partial class NewTag : FieldsController
    {
        private MainWindow Main;

        private Tag _tag;
        public NewTag(MainWindow main)
        {
            InitializeComponent();
            _tag = new Tag();
            Main = main;
            FieldsControl.ItemsSource = FieldsList = new ObservableCollection<Field>();
        }

        private void BtnSaveNewTag_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string name = TbName.Text;
            _tag.Label = name;
            foreach (var field in FieldsList)
            {
                _tag.AddField(field);
                Console.WriteLine(field.Content);
            }

            _tag.SerializeFields();
            Department department = Main.topNavigationPage.BtnShowDepartments.Content as Department;
            if (department != null)
                _tag.DepartmentID = department.ID;
            else
                Console.WriteLine("ERROR! Department not found.");

            TagRepository rep = new TagRepository();
            rep.Insert(_tag);
            Main.ChangeSourceRequest(new Tags(Main));
        }
    }
}