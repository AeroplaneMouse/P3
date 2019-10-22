using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using Asset_Management_System.Logging;

namespace Asset_Management_System.ViewModels
{
    public class AssetsViewModel
    {
        #region Constructors

        public AssetsViewModel(MainViewModel main)
        {
            // Saving reference to the main window
            _main = main;
            Search();

            // Initializing commands
            AddNewCommand = new ViewModels.Base.RelayCommand(() => _main.ChangeMainContent(new Views.AssetManager(_main)));
            SearchCommand = new ViewModels.Base.RelayCommand(() => Search());
            EditCommand = new ViewModels.Base.RelayCommand(() => Edit());
            RemoveCommand = new ViewModels.Base.RelayCommand(() => Remove());
            PrintCommand = new Base.RelayCommand(() => Print());
            ViewLogCommand = new Base.RelayCommand(() => ViewLog());
        }

        #endregion

        #region Private Properties

        private MainViewModel _main;

        #endregion

        #region Public Properties

        public string SearchQueryText { get; set; } = "";
        public int SelectedItemIndex { get; set; }
        public int ViewType => 1;

        private ObservableCollection<Asset> _list = new ObservableCollection<Asset>();

        public ObservableCollection<Asset> SearchList
        {
            get => _list;
            set
            {
                _list.Clear();
                foreach (Asset asset in value)
                    _list.Add(asset);
            }
        }

        #endregion

        #region Methods

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
            Asset selectedAsset = GetSelectedItem();
            if (selectedAsset == null)
                Console.WriteLine("An asset is not selected!");
            else
            {
                _main.ChangeMainContent(new AssetManager(_main, selectedAsset));
            }
        }

        /// <summary>
        /// Sends a remove request to the database for the selected assets.
        /// </summary>
        private void Remove()
        {
            Asset selectedAsset = GetSelectedItem();

            if (selectedAsset == null)
            {
                string message = "Please select an item.";
                //Main.ShowNotification(null, new NotificationEventArgs(message, Brushes.Red));
                Console.WriteLine(message);
                return;
            }
            else
            {
                Console.WriteLine($"Removing {selectedAsset.Name}.");
                selectedAsset.Notify(true);
                new AssetRepository().Delete(selectedAsset);

                //Main.ShowNotification(null, new NotificationEventArgs(message, Brushes.Green));

                // Reload list
                Search();
            }
        }

        public void Print()
        {
            var dialog = new PromtForReportName("asset_report_" + DateTime.Now.ToString().Replace(@"/", "").Replace(@" ", "-").Replace(@":", "") + ".csv", "Report name:");
            if (dialog.ShowDialog() == true)
            {
                if (dialog.DialogResult == true)
                {
                    string pathToFile = dialog.InputText;

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

        public void ViewLog()
        {
            Asset selected = GetSelectedItem();
            
            var dialog = new AssetHistory(selected);
            if (dialog.ShowDialog() == true)
            {
                Console.WriteLine("Displaying History for: " + selected.Name);
            }
        }

        private Asset GetSelectedItem()
        {
            if (SearchList.Count == 0)
                return null;
            else
                return SearchList.ElementAt(SelectedItemIndex);
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        
        #endregion

        #region Commands

        public ICommand AddNewCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        public ICommand ViewLogCommand { get; set; }

        #endregion
    }
}