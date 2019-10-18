using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using Asset_Management_System.Logging;
using Asset_Management_System.Models;
using Asset_Management_System.ViewModels;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for AssetHistory.xaml
    /// </summary>
    public partial class AssetHistory : Window
    {
        public AssetHistory(Asset asset)
        {
            InitializeComponent();
            //DataContext = this;
            DataContext = new AssetHistoryViewModel(this, asset);
            //ILogRepository<Entry> rep = new LogRepository();
            //History = rep.GetLogEntries(asset.ID, asset.GetType());
            this.LabelText = "History for: " + asset.Name;
        }
        
        public string LabelText
        {
            get => Label.Text;
            set => Label.Text = value;
        }

        private void BtnCLose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AssetHistoryViewModel vm = this.DataContext as AssetHistoryViewModel;
            vm.ViewCommand.Execute(null);
        }
        
        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            AssetHistoryViewModel vm = this.DataContext as AssetHistoryViewModel;
            vm.ViewCommand.Execute(null);
        }
    }
}
