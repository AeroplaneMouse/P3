using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Events;
using Asset_Management_System.Models;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for Tags.xaml
    /// </summary>
    public partial class Tags : FieldsController
    {
        private MainWindow Main;

        public Tags(MainWindow main)
        {
            InitializeComponent();
            Main = main;
        }

        private void BtnCreateNewTag_Click(object sender, RoutedEventArgs e)
        {
            Main.ChangeSourceRequest(new TagManager(Main));
        }


        private void Tb_search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Btn_search_Click(sender, new RoutedEventArgs());
        }



        private void BtnEditTag_Click(object sender, RoutedEventArgs e)
        {
            System.Collections.IList selectedTags = LvTags.SelectedItems;
            Tag input = (selectedTags[0] as Tag);

            if (selectedTags.Count != 1)
            {
                string message = $"You have selected {selectedTags.Count}. This is not a valid amount!";
                Main.ShowNotification(sender, new NotificationEventArgs(message, Brushes.Red));
                return;
            }
            else
            {
                Main.ChangeSourceRequest(new TagManager(Main, input));
            }
        }

        private void BtnRemoveAsset_Click(object sender, RoutedEventArgs e)
        {
            System.Collections.IList selectedTags = LvTags.SelectedItems;

            if (selectedTags.Count == 0)
            {
                string message = $"You have selected {selectedTags.Count}. This is not a valid amount!";
                Main.ShowNotification(sender, new NotificationEventArgs(message, Brushes.Red));
            }
            else
            {
                foreach (Tag tag in selectedTags)
                {
                    Console.WriteLine($"Removing {tag.Label}.");
                    new TagRepository().Delete(tag);
                }

                string message;
                if (selectedTags.Count > 1)
                    message = $"Multiple assets has been removed!";
                else
                    message = $"{(selectedTags[0] as Asset).Name} has been removed!";

                Main.ShowNotification(sender, new NotificationEventArgs(message, Brushes.Green));

                // Reload list
                Btn_search_Click(sender, e);
            }
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
                Btn_search_Click(sender, new RoutedEventArgs());
        }
        
        private void Btn_search_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine();
            Console.WriteLine("Searching for: " + Tb_search.Text);
            TagRepository rep = new TagRepository();
            List<Tag> tags = rep.Search(Tb_search.Text);

            Console.WriteLine("Found: " + tags.Count.ToString());

            if (tags.Count > 0)
                Console.WriteLine("-----------");

            List<MenuItem> assetsFunc = new List<MenuItem>();
            foreach (Tag tag in tags)
            {
                Console.WriteLine(tag.Label);

                //// Creating menuItems
                //MenuItem item = new MenuItem();
                //MenuItem edit = new MenuItem() { Header = "Edit" };
                //MenuItem delete = new MenuItem() { Header = "Remove" };

                //item.Header = asset.Name;
                ////AddVisualChild(edit);
                //assetsFunc.Add(item);
            }

            LvTags.ItemsSource = tags;
        }
    }
}