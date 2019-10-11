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

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for Assets.xaml
    /// </summary>
    public partial class Assets : Page
    {
        //public static event ChangeSourceEventHandler ChangeSourceRequest;

        private MainWindow Main;

        private ObservableCollection<Asset> _list;
        public TextBox TbSearch { get; set; }
        public List<Selector> SelectedItems { get; set; } = new List<Selector>();
        public string SearchText { get; set; } = "";
        public ObservableCollection<Asset> SearchList {
            get
            {
                if (_list == null)
                    _list = new ObservableCollection<Asset>();
                return _list;
            }
            set
            {
                _list.Clear();
                foreach (Asset asset in value)
                    _list.Add(asset);
            }
        }

        // Commands
        public ICommand AddNewCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand RemoveCommand { get; set; }


        public Assets(MainWindow main)
        {
            InitializeComponent();
            Main = main;
            DataContext = this;
            AddNewCommand = new ViewModels.Base.RelayCommand(() => Main.ChangeSourceRequest(new NewAsset(Main)));
            SearchCommand = new ViewModels.Base.RelayCommand(() => Search());
            EditCommand = new ViewModels.Base.RelayCommand(() => Edit());
            RemoveCommand = new ViewModels.Base.RelayCommand(() => Remove());
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) => Search();

        private void Search()
        {
            Console.WriteLine();
            Console.WriteLine("Searching for: " + SearchText);
            AssetRepository rep = new AssetRepository();
            List<Asset> assets = rep.Search(SearchText);

            Console.WriteLine("Found: " + assets.Count.ToString());

            if (assets.Count > 0)
                Console.WriteLine("-----------");

            //List<MenuItem> assetsFunc = new List<MenuItem>();
            foreach (Asset asset in assets)
            {
                Console.WriteLine(asset.Name);

                //// Creating menuItems
                //MenuItem item = new MenuItem();
                //MenuItem edit = new MenuItem() { Header = "Edit" };
                //MenuItem delete = new MenuItem() { Header = "Remove" };

                //item.Header = asset.Name;
                ////AddVisualChild(edit);
                //assetsFunc.Add(item);
            }

            ObservableCollection<Asset> test = new ObservableCollection<Asset>();
            foreach(Asset asset in assets)
                test.Add(asset);

            SearchList = test;
        }

        private void TbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Search();
        }

        private void Edit()
        { 
            if (SelectedItems.Count != 1)
            {
                string message = $"You have selected { SelectedItems.Count }. This is not a valid amount!";
                Main.ShowNotification(null, new NotificationEventArgs(message, Brushes.Red));
                return;
            }
            else
            {
                Console.WriteLine($"Editing { SelectedItems.ElementAt(0) }.");
            }
        }

        private void Remove()
        {
            System.Collections.IList seletedAssets = null/*LvList.SelectedItems*/;

            if (seletedAssets.Count == 0)
            {
                string message = $"You have selected { seletedAssets.Count }. This is not a valid amount!";
                Main.ShowNotification(null, new NotificationEventArgs(message, Brushes.Red));
                return;
            }
            else
            {
                foreach (Asset asset in seletedAssets)
                {
                    Console.WriteLine($"Removing { asset.Name }.");
                    new AssetRepository().Delete(asset);
                }

                string message;
                if (seletedAssets.Count > 1)
                    message = $"Multiple assets has been removed!";
                else
                    message = $"{ (seletedAssets[0] as Asset).Name } has been removed!";

                Main.ShowNotification(null, new NotificationEventArgs(message, Brushes.Green));

                // Reload list
                Search();
            }
        }
    }
}