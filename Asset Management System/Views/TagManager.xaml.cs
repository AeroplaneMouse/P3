using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Asset_Management_System.Models;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Events;
using Brushes = System.Windows.Media.Brushes;
using Asset_Management_System.ViewModels;
using System.Windows;
using System.Windows.Documents;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for NewTag.xaml
    /// </summary>
    public partial class TagManager : FieldsController
    {
        private readonly MainViewModel _main;

        private readonly Tag _tag;

        public List<Tag> ParentTagsList
        {
            get
            {
                TagRepository tagRepository = new TagRepository();
                return (List<Tag>)tagRepository.GetParentTags();
            }
        }

        /// <summary>
        /// TagManager is called when creating, or editing a tag.
        /// </summary>
        /// <param name="main"></param>
        /// <param name="inputTag">Optional input, only used when editing a tag.</param>
        public TagManager(MainViewModel main, Tag inputTag = null)
        {
            InitializeComponent();
            _main = main;
            DataContext = this;
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
            bool requiredFieldsWritten = true;
            _tag.Name = TbName.Text;
            _tag.Color = Color.Text;
            _tag.FieldsList = new List<Field>();
            foreach (var field in FieldsList)
            {
                if (field.Required && field.Content == string.Empty)
                {
                    requiredFieldsWritten = false;
                }
               
                _tag.AddField(field);
                Console.WriteLine(field.Content);
            }
            if (!requiredFieldsWritten)
            {
                _main.ChangeMainContent(new TagManager(_main,_tag));
                Console.WriteLine("Please fill out the required fields.");
                return;
            }

            Department department = _main.CurrentDepartment;
            if (department != null)
            {
                _tag.DepartmentID = department.ID;
                _tag.Color = Color.Text;

                // Logging the Tag
                _tag.Notify();
                TagRepository rep = new TagRepository();
                if (_editing)
                {
                    if ((ParentTag.SelectedItem as Tag) != null)
                    {
                        _tag.ParentID = (ParentTag.SelectedItem as Tag).ID;
                    }

                    rep.Update(_tag);
                }
                else
                {
                    _tag.ParentID = (ParentTag.SelectedItem as Tag).ID;
                    rep.Insert(_tag);
                }

                _main.ChangeMainContent(new Tags(_main));
            }
            else
            {
                _main.AddNotification(new Notification("ERROR! Current department not found.", Notification.ERROR));
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
                ConsoleWriter.ConsoleWrite(field.Label + " | " + field.Content);
                FieldsList.Add(field);
            }

            TbName.Text = _tag.Name;
            Color.Text = _tag.Color;
        }
    }
}