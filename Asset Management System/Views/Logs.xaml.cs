﻿using System;
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
using Asset_Management_System.ViewModels;
using Asset_Management_System.Resources.DataModels;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for Logs.xaml
    /// </summary>
    public partial class Logs : Page
    {
        public Logs(ViewModels.MainViewModel main)
        {
            InitializeComponent();
            DataContext = new ViewModels.LogsViewModel(main, ListPageType.Log);
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LogsViewModel vm = this.DataContext as LogsViewModel;
            vm.ViewCommand.Execute(null);
        }
    }
}
