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
using AMS.Controllers;
using AMS.Database.Repositories;
using AMS.IO;

namespace AMS.Views
{
    /// <summary>
    /// Interaction logic for AssetListView.xaml
    /// </summary>
    public partial class AssetListView : Page
    {
        public AssetListView()
        {
            InitializeComponent();
            DataContext = new ViewModels.AssetListViewModel(new AssetListController(new AssetRepository(), new Exporter()));
        }
    }
}
