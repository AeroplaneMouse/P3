using System;
using System.Windows;
using System.Windows.Controls;
using Asset_Management_System.Models;
using Asset_Management_System.Database.Repositories;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using Asset_Management_System.Events;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for Assets.xaml
    /// </summary>
    public partial class Assets : Page
    {
        //public static event ChangeSourceEventHandler ChangeSourceRequest;
        private MainWindow Main;

        public Assets(MainWindow main)
        {
            InitializeComponent();
            Main = main;
        }

        private void Btn_AddNewAsset_Click(object sender, RoutedEventArgs e)
        {
            Main.ChangeSourceRequest(new NewAsset(Main));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Load assets from database
            AssetRepository rep = new AssetRepository();
            List<Asset> assets = rep.Search("");
            LV_assetList.ItemsSource = assets;
        }

        private void Btn_search_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine();
            Console.WriteLine("Searching for: " + Tb_search.Text);
            AssetRepository rep = new AssetRepository();
            List<Asset> assets = rep.Search(Tb_search.Text);

            Console.WriteLine("Found: " + assets.Count.ToString());

            if (assets.Count > 0)
                Console.WriteLine("-----------");

            List<MenuItem> assetsFunc = new List<MenuItem>();
            foreach (Asset asset in assets)
            {
                Console.WriteLine(asset.Name);

                //// Creating menuItems
                //MenuItem item = new MenuItem();
                //MenuItem edit = new MenuItem() { Header = "Edit" };
                //MenuItem delete = new MenuItem() { Header = "Remove" };

                //item.Header = asset.Name;
                ////AddVisualChild(edit);
                //assetsFunc.Add(item);



            }

            LV_assetList.ItemsSource = assets;
        }

        private void Lv_mouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Asset selectedAsset = LV_assetList.SelectedItem as Asset;
            Console.WriteLine(selectedAsset?.Name);
        }

        private void Tb_search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Btn_search_Click(sender, new RoutedEventArgs());
        }

        private void BtnEditAsset_Click(object sender, RoutedEventArgs e)
        {
            System.Collections.IList seletedAssets = LV_assetList.SelectedItems;

            if (seletedAssets.Count != 1)
            {
                string message = $"You have selected { seletedAssets.Count }. This is not a valid amount!";
                Main.ShowNotification(sender, new NotificationEventArgs(message, Brushes.Red));
                return;
            }
            else
            {
                Console.WriteLine($"Editing { (seletedAssets[0] as Asset).Name }.");
            }

        }

        private void BtnRemoveAsset_Click(object sender, RoutedEventArgs e)
        {
            System.Collections.IList seletedAssets = LV_assetList.SelectedItems;

            if (seletedAssets.Count == 0)
            {
                string message = $"You have selected { seletedAssets.Count }. This is not a valid amount!";
                Main.ShowNotification(sender, new NotificationEventArgs(message, Brushes.Red));
                return;
            }
            else
            {
                foreach (Asset asset in seletedAssets)
                {
                    Console.WriteLine($"Removing { asset.Name }.");
                    new AssetRepository().Delete(asset);
                }

                string message;
                if (seletedAssets.Count > 1)
                    message = $"Multiple assets has been removed!";
                else
                    message = $"{ (seletedAssets[0] as Asset).Name } has been removed!";

                Main.ShowNotification(sender, new NotificationEventArgs(message, Brushes.Green));

                // Reload list
                Btn_search_Click(sender, e);
            }
        }
    }
}