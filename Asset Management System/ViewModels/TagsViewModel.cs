using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Asset_Management_System.Helpers;
using Asset_Management_System.Resources;
using Asset_Management_System.Views;

namespace Asset_Management_System.ViewModels
{
    public class TagsViewModel : Base.BaseViewModel
    {
        #region Constructors

        public TagsViewModel(MainViewModel main)
        {
            _main = main;

            Search();
            // Initializing commands
            AddNewCommand = new ViewModels.Base.RelayCommand(() => _main.ChangeMainContent(new Views.TagManager(_main)));
            SearchCommand = new ViewModels.Base.RelayCommand(() => Search());
            EditCommand = new ViewModels.Base.RelayCommand(() => Edit());
            RemoveCommand = new ViewModels.Base.RelayCommand(() => Remove());
            PrintCommand = new Base.RelayCommand(() => Print());
            ViewLogCommand = new Base.RelayCommand(() => ViewLog());
        }

        #endregion

        #region Private Properties

        private MainViewModel _main;

        private int _viewType;

        private ObservableCollection<Tag> _list = new ObservableCollection<Tag>();

        #endregion

        #region Public Properties

        public int ViewType => 2;
        public string SearchQueryText { get; set; } = "";
        public int SelectedItemIndex { get; set; }

        

        public ObservableCollection<Tag> SearchList
        {
            get => _list;
            set
            {
                _list.Clear();
                foreach (Tag tag in value)
                {
                    _list.Add(tag);
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
        public ICommand ViewLogCommand { get; set; }

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

            Console.WriteLine("Found: " + tags.Count);

            if (tags.Count > 0)
                Console.WriteLine("-----------");

            //List<MenuItem> assetsFunc = new List<MenuItem>();
            foreach (Tag tag in tags)
            {
                Console.WriteLine(tag.Name);

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
            Tag selectedTag = GetSelectedItem();
            if (selectedTag != null)
            {
                Console.WriteLine("Editing " + selectedTag.Name);
                _main.ChangeMainContent(new TagManager(_main, selectedTag));
            }
            else
            {
                Console.WriteLine("Please select an item");
            }
        }

        /// <summary>
        /// Sends a remove request to the database for the selected assets.
        /// </summary>
        private void Remove()
        {
            Tag selectedTag = GetSelectedItem();

            if (selectedTag == null)
            {
                string message = "Please select an item.";
                Console.WriteLine(message);
            }
            else
            {
                Console.WriteLine($"Removing {selectedTag.Name}.");
                selectedTag.Notify(true);
                new TagRepository().Delete(selectedTag);

                // Reload list
                Search();
            }
        }

        public void Print()
        {
            PrintHelper.Print(SearchList.ToList());
        }

        private Tag GetSelectedItem()
        {
            if (SearchList.Count == 0)
                return null;
            else
                return SearchList.ElementAt(SelectedItemIndex);
        }
        
        public void ViewLog()
        {
            Tag selected = GetSelectedItem();
            
            var dialog = new AssetHistory(selected);
            if (dialog.ShowDialog() == true)
            {
                Console.WriteLine("Displaying History for: " + selected.Name);
            }
        }
        
        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            throw  new  NotImplementedException();
        }


        #endregion
    }
}