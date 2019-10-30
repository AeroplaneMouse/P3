using Asset_Management_System.Helpers;
using Asset_Management_System.Logging;
using Asset_Management_System.Resources.DataModels;
using Asset_Management_System.Resources.Interfaces;
using Asset_Management_System.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels
{
    public abstract class ListPageViewModel<RepositoryType, T> : Base.BaseViewModel, IListUpdate
        where RepositoryType : Database.Repositories.ISearchableRepository<T>, new()
        where T : class, new()
    {
        #region Private Members

        private MainViewModel _main;

        private ObservableCollection<T> _list { get; set; }

        private string _searchQueryText { get; set; }

        private RepositoryType _rep { get; set; }

        private ListPageType _pageType { get; set; }

        protected MainViewModel Main
        {
            get => _main;
            set => _main = value;
        }

        protected RepositoryType Rep
        {
            get => _rep;
            set => _rep = value;
        }

        #endregion

        #region Public Properties

        public string SearchQueryText
        {
            get => _searchQueryText;
            set => _searchQueryText = value;
        }

        public ListPageType PageType
        {
            get => _pageType;
            set => _pageType = value;
        }

        public int SelectedItemIndex { get; set; }

        public ObservableCollection<T> SearchList
        {
            get => _list;
            set
            {
                _list.Clear();

                foreach (T item in value)
                {
                    _list.Add(item);
                }
            }
        }

        #endregion

        #region Commands

        public ICommand SearchCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        public ICommand ViewCommand { get; set; }

        #endregion

        #region Constructor

        public ListPageViewModel(MainViewModel main, ListPageType pageType)
        {
            _rep = new RepositoryType();

            _main = main;

            _searchQueryText = String.Empty;
            _list = new ObservableCollection<T>();

            _pageType = pageType;


            // Initialize commands
            PrintCommand = new Base.RelayCommand(Print);
            SearchCommand = new Base.RelayCommand(Search);
            ViewCommand = new Base.RelayCommand(View);
        }

        #endregion

        #region Methods

        public void UpdateList()
        {
            Search();
        }

        /// <summary>
        /// Sends a search request to the database, and sets the list of items to the result.
        /// </summary>
        protected virtual void Search()
        {
            Console.WriteLine();
            Console.WriteLine("Searching for: " + SearchQueryText);

            ObservableCollection<T> items = _rep.Search(SearchQueryText);

            Console.WriteLine("Found: " + items.Count.ToString() + " items");

            SearchList = items;
        }

        /// <summary>
        /// Creates a csv file containing all the items
        /// </summary>
        protected void Print()
        {
            PrintHelper.Print(SearchList as IEnumerable<object>);
        }

        /// <summary>
        /// Displays the selected item
        /// </summary>
        protected abstract void View();

        #endregion


        #region Helpers


        protected T GetSelectedItem()
        {
            if (SearchList.Count == 0)
                return null;

            else
            {
                return SearchList.ElementAt(SelectedItemIndex);
            }
        }

        #endregion
    }
}
