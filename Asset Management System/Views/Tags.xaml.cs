using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for Tags.xaml
    /// </summary>
    public partial class Tags : Page
    {
        private MainWindow Main;
        public Tags(MainWindow main)
        {
            InitializeComponent();
            Main = main;
        }

        private void BtnCreateNewTag_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            TagRepository rep = new TagRepository();
            List<Tag> tags = rep.Search(TbSearch.Text);
            LvTags.ItemsSource = tags;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Load all tags
            TagRepository rep = new TagRepository();
            List<Tag> tags = rep.Search("");
            LvTags.ItemsSource = tags;
        }

        private void TbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                BtnSearch_Click(sender, new RoutedEventArgs());
        }
    }
}
