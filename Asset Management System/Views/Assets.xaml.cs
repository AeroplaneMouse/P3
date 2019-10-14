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

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for Assets.xaml
    /// </summary>
    public partial class Assets : Page
    {
        private MainViewModel _main;

        public Assets(MainViewModel main)
        {
            InitializeComponent();
            _main = main;
            DataContext = new ViewModels.AssetsViewModel(_main);
        }

        //private void Page_Loaded(object sender, RoutedEventArgs e) => Search();
    }
}