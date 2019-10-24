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
using Asset_Management_System.Helpers;
using Asset_Management_System.Logging;

namespace Asset_Management_System.ViewModels
{
    public class AssetsViewModel : Base.BaseViewModel
    {
        #region Constructors

        public AssetsViewModel(MainViewModel main)
        {
            _assetRep = new AssetRepository();

            // Saving reference to the main window
            _main = main;
            Search();

            // Initializing commands
            AddNewCommand = new ViewModels.Base.RelayCommand(() => _main.ChangeMainContent(new NewAssetManager(main)));
            SearchCommand = new ViewModels.Base.RelayCommand(() => Search());
            EditCommand = new ViewModels.Base.RelayCommand(() => Edit());
            RemoveCommand = new ViewModels.Base.RelayCommand(() => Remove());
            PrintCommand = new Base.RelayCommand(() => Print());
            ViewLogCommand = new Base.RelayCommand(() => ViewLog());
        }

        #endregion

        #region Private Properties

        private MainViewModel _main;

        private ObservableCollection<Asset> _list = new ObservableCollection<Asset>();

        private AssetRepository _assetRep { get; set; }

        #endregion

        #region Public Properties

        public string SearchQueryText { get; set; } = "";
        public int SelectedItemIndex { get; set; }
        public int ViewType => 1;

        public ObservableCollection<Asset> SearchList
        {
            get => _list;
            set
            {
                _list.Clear();
                foreach (Asset asset in value)
                {
                    _list.Add(asset);
                }       
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
            ObservableCollection<Asset> assets = _assetRep.Search(SearchQueryText);

            Console.WriteLine("Found: " + assets.Count.ToString());

            if (assets.Count > 0)
                Console.WriteLine("-----------");

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
                _main.ChangeMainContent(new NewAssetManager(_main, selectedAsset));
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
                _assetRep.Delete(selectedAsset);

                //Main.ShowNotification(null, new NotificationEventArgs(message, Brushes.Green));

                // Reload list
                Search();
            }
        }

        public void Print()
        {
            PrintHelper.Print(SearchList.ToList());
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