using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;

namespace Asset_Management_System.Pages
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        public Settings()
        {
            InitializeComponent();

            List<int> tags = new List<int>();
            tags.Add(5);

            AssetRepository rep = new AssetRepository();
            List<Asset> assetsByTags = rep.SearchByTags(tags);

            foreach(Asset asset in assetsByTags){
                Console.WriteLine(asset.Label);
            }
        }
    }
}
