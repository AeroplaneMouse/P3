﻿using AMS.ViewModels;
using AMS.Interfaces;
using AMS.Controllers;
using System.Windows.Controls;
using AMS.Database.Repositories.Interfaces;
using System.Collections;
using AMS.Models;
using System.Windows;

namespace AMS.Views
{
    /// <summary>
    /// Interaction logic for AssetListView.xaml
    /// </summary>
    public partial class AssetList : Page
    {
        public AssetList(MainViewModel main, IAssetRepository assetRepository, IExporter exporter)
        {
            InitializeComponent();
            DataContext = new AssetListViewModel(main, new AssetListController(assetRepository, exporter));
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AssetListViewModel viewModel = (DataContext as AssetListViewModel);
            viewModel.SelectedItems.Clear();

            foreach (Asset asset in (sender as ListView).SelectedItems)
                viewModel.SelectedItems.Add(asset);

            // Setting single item selected visibility
            viewModel.SingleSelected = viewModel.SelectedItems.Count == 1 ?
                Visibility.Visible :
                Visibility.Collapsed;

            // Setting multiple item selected visibility
            viewModel.MultipleSelected = viewModel.SelectedItems.Count > 0 ?
                    Visibility.Visible :
                    Visibility.Collapsed;
        }
    }
}
