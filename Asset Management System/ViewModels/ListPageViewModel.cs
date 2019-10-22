using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels
{
    public abstract class ListPageViewModel<RepositoryType, T> : Base.BaseViewModel
        where RepositoryType : Database.Repositories.ISearchableRepository<T>, new()
        where T : new()
    {
        #region Private Members

        private MainViewModel _main;

        private ObservableCollection<T> _list { get; set; }

        private string _searchQueryText { get; set; }

        private RepositoryType _rep { get; set; }

        #endregion


        #region Public Properties

        public string SearchQueryText
        {
            get => _searchQueryText;
            set => _searchQueryText = value;
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

        public ICommand AddNewCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        //public ICommand ViewLogCommand { get; set; }

        #endregion


        #region Constructor

        public ListPageViewModel(MainViewModel main)
        {
            _rep = new RepositoryType();

            _main = main;

            //Search();

            _searchQueryText = String.Empty;


            // Initialize commands
            //AddNewCommand = new Base.RelayCommand(() => _main.ChangeMainContent(new TManager(_main)));


            // Search
            // Print
            // View



        }

        #endregion


        #region Methods

        protected virtual void Search()
        {
            ObservableCollection<T> items = _rep.Search(SearchQueryText);
        }
        

        #endregion
    }
}
