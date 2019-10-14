using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels
{
    public class TagsViewModel : Base.BaseViewModel
    {
        #region Constructors

        public TagsViewModel(MainViewModel main)
        {
            _main = main;

            // Initializing commands
            AddNewCommand = new ViewModels.Base.RelayCommand(() => _main.ChangeMainContent(new Views.TagManager(_main)));
            SearchCommand = new ViewModels.Base.RelayCommand(() => Search());
            EditCommand = new ViewModels.Base.RelayCommand(() => Edit());
            RemoveCommand = new ViewModels.Base.RelayCommand(() => Remove());
            PrintCommand = new Base.RelayCommand(() => Print());
        }
        #endregion

        #region Private Properties

        private MainViewModel _main;

        #endregion

        #region Public Properties

        public string SearchQueryText { get; set; } = "";
        public int SelectedItemIndex { get; set; }

        private ObservableCollection<Tag> _list;

        public ObservableCollection<Tag> SearchList
        {
            get
            {
                if (_list == null)
                    _list = new ObservableCollection<Tag>();
                return _list;
            }
            set
            {
                _list.Clear();
                foreach (Tag tag in value)
                    _list.Add(tag);
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

        #region Methods

        /// <summary>
        /// Sends a search request to the database, and sets the list of assets to the result.
        /// </summary>
        private void Search()
        {
            Console.WriteLine();
            Console.WriteLine("Searching for: " + SearchQueryText);
            ObservableCollection<Tag> tags = new TagRepository().Search(SearchQueryText);

            Console.WriteLine("Found: " + tags.Count.ToString());

            if (tags.Count > 0)
                Console.WriteLine("-----------");

            //List<MenuItem> assetsFunc = new List<MenuItem>();
            foreach (Tag tag in tags)
            {
                Console.WriteLine(tag.Label);

                //// Creating menuItems
                //MenuItem item = new MenuItem();
                //MenuItem edit = new MenuItem() { Header = "Edit" };
                //MenuItem delete = new MenuItem() { Header = "Remove" };

                //item.Header = asset.Name;
                ////AddVisualChild(edit);
                //assetsFunc.Add(item);
            }

            SearchList = tags;
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

        public void Print()
        {
            throw new NotImplementedException();
        }

        private Tag GetSelectedItem()
        {
            if (SearchList.Count == 0)
                return null;
            else
                return SearchList.ElementAt(SelectedItemIndex);
        }

        #endregion
    }
}
