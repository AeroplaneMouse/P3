using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Asset_Management_System.Models;
using Asset_Management_System.Database.Repositories;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using Asset_Management_System.Events;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using Asset_Management_System.ViewModels;
using Asset_Management_System.Resources.DataModels;
using Asset_Management_System.Services.Interfaces;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for Assets.xaml
    /// </summary>
    public partial class Assets : Page
    {
        public Assets(MainViewModel main, IAssetService assetService)
        {
            InitializeComponent();

            DataContext = new ViewModels.AssetsViewModel(main, assetService);
        }
    }
}