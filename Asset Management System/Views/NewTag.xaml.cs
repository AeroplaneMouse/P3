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
    ///// <summary>
    ///// Interaction logic for NewTag.xaml
    ///// </summary>
    //public partial class NewTag : FieldsController
    //{
    //    private MainWindow Main;

    //    private Tag _tag;

    //    public NewTag(MainWindow main)
    //    {
    //        InitializeComponent();
    //        _tag = new Tag();
    //        Main = main;
    //        FieldsControl.ItemsSource = FieldsList = new ObservableCollection<Field>();
    //    }

    //    private void BtnSaveNewTag_Click(object sender, System.Windows.RoutedEventArgs e)
    //    {
    //        _tag.Label = TbName.Text;
    //        _tag.Color = Color.Text;
    //        foreach (var field in FieldsList)
    //        {
    //            _tag.AddField(field);
    //            Console.WriteLine(field.Content);
    //        }

    //        _tag.SerializeFields();
    //        Department department = Main.topNavigationPage.SelectedDepartment;
    //        if (department != null)
    //        {
    //            _tag.DepartmentID = department.ID;
    //            TagRepository rep = new TagRepository();
    //            rep.Insert(_tag);
    //            Main.ChangeMainContent(new Tags(Main));
    //        }
    //        else
    //        {
                
    //            Main.ShowNotification(null,new NotificationEventArgs("Department not selected",Brushes.Red));
    //            Console.WriteLine("ERROR! Department not found.");
    //        }
    //    }

    //    private void ColorPickerColorChanged(object sender, EventArgs e)
    //    {
    //        Color.Text = ColorPicker.SelectedColor.ToString();
    //    }
    //}
}