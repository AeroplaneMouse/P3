﻿using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using Asset_Management_System.Models;
using Asset_Management_System.ViewModels;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for NewTag.xaml
    /// </summary>
    public partial class TagManager : Page
    {

        /// <summary>
        /// TagManager is called when creating, or editing a tag.
        /// </summary>
        /// <param name="main"></param>
        /// <param name="inputTag">Optional input, only used when editing a tag.</param>
        public TagManager(MainViewModel main, Tag inputTag = null)
        {
            InitializeComponent();
            DataContext = new TagManagerViewModel(main, inputTag);
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            TagManagerViewModel viewModel = DataContext as TagManagerViewModel;

            Color c = (Color)e.NewValue;

            string hexColor = $"#{ToHexStr(c.R)}{ToHexStr(c.G)}{ToHexStr(c.B)}";
            viewModel.Color = hexColor;
        }

        private string ToHexStr(int i)
        {
            string hex = i.ToString("X");
            if (hex.Length == 1)
                hex = $"0{ hex }";
            return hex;
        }
    }
}