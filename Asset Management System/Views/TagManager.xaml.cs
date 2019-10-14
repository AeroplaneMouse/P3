using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using Asset_Management_System.Models;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Events;
using Brushes = System.Windows.Media.Brushes;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for NewTag.xaml
    /// </summary>
    public partial class TagManager : FieldsController
    {
        private MainWindow Main;

        private Tag _tag;

        private bool _editing;

        public TagManager(MainWindow main,Tag inputTag = null)
        {
            InitializeComponent();
            Main = main;
            FieldsControl.ItemsSource = FieldsList = new ObservableCollection<Field>();
            if (inputTag != null)
            {
                _tag = inputTag;
                _editing = true;
                LoadFields();
            }
            else
            {
                _tag = new Tag();
                _editing = false;
            }
        }

        private void BtnSaveNewTag_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _tag.Label = TbName.Text;
            _tag.Color = Color.Text;
            foreach (var field in FieldsList)
            {
                _tag.AddField(field);
                Console.WriteLine(field.Content);
            }

            _tag.SerializeFields();
            Department department = Main.topNavigationPage.SelectedDepartment;
            if (department != null)
            {
                _tag.DepartmentID = department.ID;
                _tag.Color = Color.Text;
                TagRepository rep = new TagRepository();
                if (_editing)
                {
                    rep.Update(_tag);
                }
                else
                {
                    rep.Insert(_tag);
                }
                
                Main.ChangeSourceRequest(new Tags(Main));
            }
            else
            {
                
                Main.ShowNotification(null,new NotificationEventArgs("Department not selected",Brushes.Red));
                Console.WriteLine("ERROR! Department not found.");
            }
        }

        private void ColorPickerColorChanged(object sender, EventArgs e)
        {
            Color.Text = ColorPicker.SelectedColor.ToString();
        }
        
        private bool LoadFields()
        {
            ConsoleWriter.ConsoleWrite("------Field labels | Field content -------");
            _tag.DeserializeFields();
            foreach (var field in _tag.FieldsList)
            {
                ConsoleWriter.ConsoleWrite(field.Label +" | "+ field.Content);
                FieldsList.Add(field);
            }
            TbName.Text = _tag.Label;
            Color.Text = _tag.Color;
            return true;
        }
    }
}