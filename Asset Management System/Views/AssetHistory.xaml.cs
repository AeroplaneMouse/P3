using System;
using System.Windows;
using System.Windows.Input;
using Asset_Management_System.Models;
using Asset_Management_System.Services.Interfaces;
using Asset_Management_System.ViewModels;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for AssetHistory.xaml
    /// </summary>
    public partial class AssetHistory : Window
    {
        public AssetHistory(Asset asset, IEntryService entryService)
        {
            InitializeComponent();
            //DataContext = this;
            DataContext = new AssetHistoryViewModel(this, entryService, asset);
            //ILogRepository<Entry> rep = new LogRepository();
            //History = rep.GetLogEntries(asset.ID, asset.GetType());
            Type type = asset.GetType();
            this.LabelText = "History for: " + type.Name + " " + type.GetProperty("Name")?.GetValue(asset).ToString();
        }
        
        public string LabelText
        {
            get => Label.Text;
            set => Label.Text = value;
        }

        //TODO Figure out how to remove this, and instead bind it to the ViewCommand in the viewModel.
        // ^ Don't think it's possible, MouseBinding doesn't work in ListViews
        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AssetHistoryViewModel vm = this.DataContext as AssetHistoryViewModel;
            vm.ViewCommand.Execute(null);
        }
    }
}
