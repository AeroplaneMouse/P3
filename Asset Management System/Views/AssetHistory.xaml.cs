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
            DataContext = this;
            ILogRepository<Entry> rep = new LogRepository();
            //foreach (Entry entry in rep.GetLogEntries(asset.ID, asset.GetType())) History.Add(entry);
            History = rep.GetLogEntries(asset.ID, asset.GetType());
            this.LabelText = "History for: " + asset.Name;
        }
        
        public string LabelText
        {
            get => Label.Text;
            set => Label.Text = value;
        }
        
        public int SelectedItemIndex { get; set; }
        
        /*
        private ObservableCollection<Entry> _list = new ObservableCollection<Entry>();
        public ObservableCollection<Entry> History
        {
            get => _list;
            set
            {
                _list.Clear();
                foreach (Entry entry in value)
                    _list.Add(entry);
            }
        }
        /**/
        public List<Entry> History { get; set; }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ViewLogEntry();
        }
        
        /// <summary>
        /// Displays the selected log entry
        /// </summary>
        private void ViewLogEntry()
        {
            Entry selected = GetSelectedItem();
            var dialog = new ShowEntry(selected);
            if (dialog.ShowDialog() == true)
            {
                Console.WriteLine("Displaying log entry saying : " + selected.Description);
            }
        }
        
        private Entry GetSelectedItem()
        {
            if (History.Count == 0)
                return null;
            else
                return History.ElementAt(SelectedItemIndex);
        }
    }
}
