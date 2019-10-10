﻿using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Asset_Management_System.Models;
using Asset_Management_System.Database.Repositories;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for NewAsset.xaml
    /// </summary>
    public partial class NewAsset
    {
        private Asset _asset;

        private MainWindow Main;

        private int id = 0;

        public ObservableCollection<Field> FieldsList { get; set; }


        public NewAsset(MainWindow main)
        {
            InitializeComponent();
            _asset = new Asset();
            Main = main;
            FieldsControl.ItemsSource = FieldsList = new ObservableCollection<Field>();
        }

        private void BtnSaveNewAsset_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string name = TbName.Text;
            string description = TbDescription.Text;
            _asset.Name = name;
            _asset.Description = description;
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

            AssetRepository rep = new AssetRepository();
            rep.Insert(_asset);
            Main.ChangeSourceRequest(new Home(Main));
        }

        /// <summary>
        /// Function to add fields to the list of fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotSupportedException"></exception>
        private void OnAddField(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button)?.Name)
            {
                case "AddTextField":
                    Console.WriteLine("Textfield added");
                    FieldsList.Add(new Field(id++, "TextField", "", 1, ""));
                    break;
                case "AddStringField":
                    Console.WriteLine("StringField added");
                    FieldsList.Add(new Field(id++, "StringField", "", 2, ""));
                    break;
                case "AddIntegerField":
                    Console.WriteLine("IntegerField added");
                    FieldsList.Add(new Field(id++, "Integer", "", 3, ""));
                    break;
                case "AddDateField":
                    Console.WriteLine("DataField added");
                    FieldsList.Add(new Field(id++, "DateField", "", 4, ""));
                    break;
                case "AddBooleanField":
                    Console.WriteLine("BooleanField added");
                    FieldsList.Add(new Field(id++, "BooleanField", "", 5, ""));
                    break;
                default:
                    throw new NotSupportedException();
            }

            Console.WriteLine("List of the current fields after adding the field:");
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("ID  |   Field name   |   Content of the field");
            foreach (var test in FieldsList)
            {

                Console.WriteLine(test.id + " | " + test.Label + " | " + test.Content);
                


            }

            Console.WriteLine("---------------------------------------");
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
                Console.WriteLine(test.id + " | " +test.Label + "|" + test.Content);
                
            }

            Console.WriteLine("---------------------------------------");
        }
    }

    public class FieldDesignSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            Field field = item as Field;
            switch (field.FieldType)
            {
                case 1: // Textbox
                    return element.FindResource("TextBoxFieldStyle") as DataTemplate;
                case 2: // String
                    return element.FindResource("StringFieldStyle") as DataTemplate;
                case 3: // Integer
                    return element.FindResource("IntegerFieldStyle") as DataTemplate;
                case 4: // Date
                    return element.FindResource("DateFieldStyle") as DataTemplate;
                case 5: // Boolean
                    return element.FindResource("BooleanFieldStyle") as DataTemplate;
                default:
                    throw new NotSupportedException("Wrong formatting syntax.");
            }
        }
    }
}
