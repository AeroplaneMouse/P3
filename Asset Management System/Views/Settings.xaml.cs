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

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        private MainWindow Main;
        public Settings(MainWindow main)
        {
            InitializeComponent();
            Main = main;

            TagRepository tag_rep = new TagRepository();
            List<Tag> tags = tag_rep.GetParentTags();

            AssetRepository asset_rep = new AssetRepository();
            Asset asset = asset_rep.GetById(3);

            asset_rep.AttachTagsToAsset(asset, tags);
        }
    }
}
