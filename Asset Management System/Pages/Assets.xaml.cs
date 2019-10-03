using System;
using System.Windows;
using System.Windows.Controls;
using Asset_Management_System.Database;
using Asset_Management_System.Events;
using Asset_Management_System.Models;
using Asset_Management_System.Database.Repositories;
using System.Collections.Generic;

namespace Asset_Management_System.Pages
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

            //DBConnection db = DBConnection.Instance();
            //if(db.IsConnect())
            //{

            //}

            //LV_assetList.ItemsSource = 
        }

        private void Btn_AddNewAsset_Click(object sender, RoutedEventArgs e)
        { 
            Main.ChangeSourceRequest(this, new ChangeSourceEventArgs(new NewAsset()));
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
            Console.WriteLine("Searching for: "+Tb_search.Text);
            AssetRepository rep = new AssetRepository();
            List<Asset> assets = rep.Search(Tb_search.Text);

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
    }
}
