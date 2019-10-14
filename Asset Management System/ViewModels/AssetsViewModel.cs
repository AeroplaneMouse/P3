using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels
{
    public class AssetsViewModel
    {
        #region Constructors

        public AssetsViewModel(MainViewModel main)
        {
            // Saving reference to the main window
            _main = main;

            // Initializing commands
            AddNewCommand = new ViewModels.Base.RelayCommand(() => _main.ChangeMainContent(new NewAsset(_main)));
            SearchCommand = new ViewModels.Base.RelayCommand(() => Search());
            EditCommand = new ViewModels.Base.RelayCommand(() => Edit());
            RemoveCommand = new ViewModels.Base.RelayCommand(() => Remove());
            PrintCommand = new Base.RelayCommand(() => Print());
        }

        #endregion

        #region Private Properties

        private MainViewModel _main;

        /// <summary>
        /// Sends a search request to the database, and sets the list of assets to the result.
        /// </summary>
        private void Search()
        {
            Console.WriteLine();
            Console.WriteLine("Searching for: " + SearchQueryText);
            AssetRepository rep = new AssetRepository();
            ObservableCollection<Asset> assets = rep.Search(SearchQueryText);

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

            SearchList = assets;
        }

        /// <summary>
        /// Opens the edit view for the selected asset.
        /// </summary>
        private void Edit()
        {
            //System.Collections.IList seletedAssets = LV_assetList.SelectedItems;
            //Asset input = (seletedAssets[0] as Asset);

            if (SelectedItems.Count != 1)
            {
                string message = $"You have selected { SelectedItems.Count }. This is not a valid amount!";
                //Main.ShowNotification(null, new NotificationEventArgs(message, Brushes.Red));
                Console.WriteLine(message);
                return;
            }
            else
            {
                //Main.ChangeMainContent(new EditAsset(Main,input));
                //Console.WriteLine($"Editing { SelectedItems.ElementAt(0) }.");
                Console.WriteLine("Editing the selected item.");
            }
        }

        /// <summary>
        /// Sends a remove request to the database for the selected assets.
        /// </summary>
        private void Remove()
        {
            System.Collections.IList seletedAssets = null/*LvList.SelectedItems*/;

            if (seletedAssets.Count == 0)
            {
                string message = $"You have selected { seletedAssets.Count }. This is not a valid amount!";
                //Main.ShowNotification(null, new NotificationEventArgs(message, Brushes.Red));
                Console.WriteLine(message);
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

                //Main.ShowNotification(null, new NotificationEventArgs(message, Brushes.Green));
                Console.WriteLine(message);

                // Reload list
                Search();
            }
        }

        #endregion

        #region Public Properties

        public string SearchQueryText { get; set; } = "";

        public List<Selector> SelectedItems { get; set; } = new List<Selector>();
        
        private ObservableCollection<Asset> _list;

        public ObservableCollection<Asset> SearchList
        {
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

        #endregion

        #region Methods

        public void Print()
        {
            var dialog = new PromtForReportName();
            if (dialog.ShowDialog() == true)
            {
                if (dialog.DialogResult == true)
                {
                    string pathToFile = dialog.ResponseText;

                    if (!pathToFile.EndsWith(".csv"))
                    {
                        pathToFile = pathToFile + ".csv";
                    }

                    pathToFile = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + pathToFile;

                    using (StreamWriter file = new StreamWriter(pathToFile, false))
                    {
                        foreach (Asset asset in SearchList)
                        {
                            string fileEntry = asset.ID + "," + asset.Name;
                            file.WriteLine(fileEntry);
                        }
                    }
                }
            }
        }

        #endregion

        #region Commands

        public ICommand AddNewCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand RemoveCommand { get; set; }

        public ICommand PrintCommand { get; set; }

        #endregion
    }
}