using System;
using System.Windows;
using System.Windows.Controls;
using Asset_Management_System.Database;
using Asset_Management_System.Events;
using Asset_Management_System.Models;
using Asset_Management_System.Database.Repositories;
using System.Collections.Generic;
using System.Windows.Input;

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
            Console.WriteLine("Searching for: "+Tb_search.Text);
            AssetRepository rep = new AssetRepository();
            List<Asset> assets = rep.Search(Tb_search.Text);

            Console.WriteLine("Found: "+assets.Count.ToString());

            if(assets.Count > 0)
            {
                Console.WriteLine("-----------");
            }
            
            foreach(Asset asset in assets){
                Console.WriteLine(asset.Label);
            }

            LV_assetList.ItemsSource = assets;
        }

        private void Lv_mouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Asset selectedAsset = LV_assetList.SelectedItem as Asset;
            Console.WriteLine(selectedAsset.Label);

        }

        private void Tb_search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Btn_search_Click(sender, new RoutedEventArgs());
        }
    }
}
