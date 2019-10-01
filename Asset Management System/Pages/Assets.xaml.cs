﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace Asset_Management_System.Pages
{
    /// <summary>
    /// Interaction logic for Assets.xaml
    /// </summary>
    public partial class Assets : Page
    {
        public static event EventHandler ChangeSourceRequest;

        public Assets()
        {
            InitializeComponent();

        }

        private void Btn_AddNewAsset_Click(object sender, RoutedEventArgs e)
        {
            OnChangeSourceRequest(e);
        }

        void OnChangeSourceRequest(EventArgs e)
        {
            //EventHandler handler = ChangeSourceRequest;
            //if (handler != null)
            //    handler(this, e)
            ChangeSourceRequest?.Invoke(this, e);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Load assets from database
        }
    }
}
