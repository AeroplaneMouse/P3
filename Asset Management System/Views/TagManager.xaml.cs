using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Asset_Management_System.Models;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Events;
using Brushes = System.Windows.Media.Brushes;
using Asset_Management_System.ViewModels;
using System.Windows;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for NewTag.xaml
    /// </summary>
    public partial class TagManager : FieldsController
    {
        private readonly MainViewModel _main;

        private readonly Tag _tag;

        private readonly bool _editing;

        /// <summary>
        /// TagManager is called when creating, or editing a tag.
        /// </summary>
        /// <param name="main"></param>
        /// <param name="inputTag">Optional input, only used when editing a tag.</param>
        public TagManager(MainViewModel main,Tag inputTag = null)
        {
            InitializeComponent();
            _main = main;
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

        /// <summary>
        /// This function fires when the "Save Tag" button is clicked.
        /// The function saves or updates the tag in the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSaveNewTag_Click(object sender, RoutedEventArgs e)
        {
            _tag.Label = TbName.Text;
            _tag.Color = Color.Text;
            _tag.FieldsList = new List<Field>();
            foreach (var field in FieldsList)
            {
                _tag.AddField(field);
                Console.WriteLine(field.Content);
            }

            _tag.SerializeFields();
            Department department = _main.CurrentDepartment;
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
                
                _main.ChangeMainContent(new Tags(_main));
            }
            else
            {
                
                _main.ShowNotification(null,new NotificationEventArgs("Department not selected",Brushes.Red));
                Console.WriteLine("ERROR! Department not found.");
            }
        }
        
        /// <summary>
        /// This function is used for updating the textBox, with the selected color.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorPickerColorChanged(object sender, EventArgs e)
        {
            Color.Text = ColorPicker.SelectedColor.ToString();
        }
        
        /// <summary>
        /// Runs through the saved fields within the tag, and adds these to the fieldList.
        /// </summary>
        /// <returns></returns>
        private void LoadFields()
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
        }
    }
}