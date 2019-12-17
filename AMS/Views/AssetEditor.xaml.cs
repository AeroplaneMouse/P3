using AMS.Models;
using AMS.ViewModels;
using AMS.Controllers;
using System.Windows.Controls;
using AMS.Database.Repositories.Interfaces;
using AMS.Controllers.Interfaces;
using AMS.Helpers;
using System.Windows;
using System;

namespace AMS.Views
{
    public partial class AssetEditor : Page
    {
        public AssetEditor(IAssetController assetController, TagHelper tagHelper)
        {
            InitializeComponent();
            DataContext = new AssetEditorViewModel(assetController, tagHelper);
            SearchElement.GotFocus += tagInputBoxFocus;
            SearchElement.LostFocus += tagInputBoxFocusLost;
        }

        private void tagInputBoxFocusLost(object sender, RoutedEventArgs e)
        {
            TagSuggestionPopup.IsOpen = false;
        }

        private void tagInputBoxFocus(object sender, RoutedEventArgs e)
        {
            ((AssetEditorViewModel)DataContext).TagSearch();
            TagSuggestionPopup.IsOpen = true;
        }
    }
}