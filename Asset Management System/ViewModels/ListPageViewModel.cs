using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Generic;
using Asset_Management_System.Views;
using System.Collections.ObjectModel;
using Asset_Management_System.Models;
using Asset_Management_System.Logging;
using Asset_Management_System.Helpers;
using Asset_Management_System.Resources.DataModels;
using Asset_Management_System.Resources.Interfaces;

namespace Asset_Management_System.ViewModels
{
    public abstract class ListPageViewModel<RepositoryType, T> : Base.BaseViewModel, IListUpdate
        where RepositoryType : Database.Repositories.ISearchableRepository<T>, new()
        where T : class, new()
    {
        protected MainViewModel _main;
        private ObservableCollection<T> _list { get; set; }
        private string _searchQueryText { get; set; }
        private RepositoryType _rep { get; set; }
        private ListPageType _pageType { get; set; }

        protected RepositoryType Rep
        {
            get => _rep;
            set => _rep = value;
        }

        public Visibility Visible
        {
            get => _main.Visible;
            private set => Visible = value;
        }

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

                OnPropertyChanged(nameof(SearchList));
            }
        }   

        public ICommand PrintCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ViewCommand { get; set; }
        public ICommand HeaderClickCommand { get; set; }

        public ListPageViewModel(MainViewModel main, ListPageType pageType)
        {
            _main = main;
            _rep = new RepositoryType();

            _searchQueryText = String.Empty;
            _list = new ObservableCollection<T>();

            _pageType = pageType;

            // Initialize commands
            PrintCommand = new Commands.PrintSelectedItemsCommand();
            SearchCommand = new Base.RelayCommand(Search);
            ViewCommand = new Base.RelayCommand(View);
            HeaderClickCommand = new Base.RelayCommand<object>(HeaderClick);
        }


        public virtual void PageFocus() 
        {
            Search();
        }

        public virtual void PageLostFocus()
        {

        }

        /// <summary>
        /// Sends a search request to the database, and sets the list of items to the result.
        /// </summary>
        protected virtual void Search() => SearchList = _rep.Search(SearchQueryText);

        /// <summary>
        /// Creates a csv file containing all the items
        /// </summary>
        protected void Print() => PrintHelper.Print(SearchList as IEnumerable<object>);

        /// <summary>
        /// Displays the selected item
        /// </summary>
        protected virtual void View()
        {
            T selected = GetSelectedItem();

            if (selected == null) return;
            switch (selected)
            {
                case Tag tag:
                    _main.ChangeMainContent(new ObjectViewer(_main, tag));
                    break;
                case Asset asset:
                    _main.ChangeMainContent(new ObjectViewer(_main, asset));
                    break;
                case Entry entry:
                    new ShowEntry(entry).ShowDialog();
                    break;
                default:
                    _main.AddNotification(
                        new Notification("ERROR! Unknown error occured when trying to view.", Notification.ERROR),
                        3000);
                    break;
            }
        }

        /// <summary>
        /// Sorts the SearchList when a GridViewColumnHeader is clicked
        /// </summary>
        /// <param name="header"></param>
        protected void HeaderClick(object header)
        {
            GridViewColumnHeader clickedHeader = header as GridViewColumnHeader;
            switch (SearchList[0])
            {
                case Asset _:
                {
                    Console.WriteLine("Sorting Assets");
                    SearchList = clickedHeader?.Content.ToString() switch
                    {
                        "Name" => new ObservableCollection<T>(SearchList.Cast<Asset>()
                            .OrderBy(i => i.Name)
                            .Cast<T>()),
                        "ID" => new ObservableCollection<T>(SearchList.Cast<Asset>()
                            .OrderBy(i => i.ID)
                            .Cast<T>()),
                        "Identifier" => new ObservableCollection<T>(SearchList.Cast<Asset>()
                            .OrderBy(i => i.Identifier)
                            .Cast<T>()),
                        "Created" => new ObservableCollection<T>(SearchList.Cast<Asset>()
                            .OrderBy(i => i.CreatedAt)
                            .Cast<T>()),
                        _ => SearchList
                    };
                    break;
                }
                case Tag _:
                {
                    Console.WriteLine("Sorting Tags");
                    SearchList = clickedHeader?.Content.ToString() switch
                    {
                        "Label" => new ObservableCollection<T>(SearchList.Cast<Tag>()
                            .OrderBy(i => i.Name)
                            .Cast<T>()),
                        "ID" => new ObservableCollection<T>(SearchList.Cast<Tag>()
                            .OrderBy(i => i.ID)
                            .Cast<T>()),
                        "Parent Tag ID" => new ObservableCollection<T>(SearchList.Cast<Tag>()
                            .OrderBy(i => i.ParentID)
                            .Cast<T>()),
                        "Department ID" => new ObservableCollection<T>(SearchList.Cast<Tag>()
                            .OrderBy(i => i.DepartmentID)
                            .Cast<T>()),
                        "Color" => new ObservableCollection<T>(SearchList.Cast<Tag>()
                            .OrderBy(i => i.Color)
                            .Cast<T>()),
                        "Created" => new ObservableCollection<T>(SearchList.Cast<Tag>()
                            .OrderBy(i => i.CreatedAt)
                            .Cast<T>()),
                        _ => SearchList
                    };
                    break;
                }
                case Entry _:
                {
                    Console.WriteLine("Sorting Log");
                    SearchList = clickedHeader?.Content.ToString() switch
                    {
                        "Date" => new ObservableCollection<T>(SearchList.Cast<Entry>()
                            .OrderBy(i => i.CreatedAt)
                            .Cast<T>()),
                        "ID" => new ObservableCollection<T>(SearchList.Cast<Entry>()
                            .OrderBy(i => i.Id)
                            .Cast<T>()),
                        "Event" => new ObservableCollection<T>(SearchList.Cast<Entry>()
                            .OrderBy(i => i.Description)
                            .Cast<T>()),
                        "User" => new ObservableCollection<T>(SearchList.Cast<Entry>()
                            .OrderBy(i => i.Username)
                            .Cast<T>()),
                        _ => SearchList
                    };
                    break;
                }
            }
        }


        protected T GetSelectedItem()
        {
            if (SearchList.Count == 0)
                return null;
            else
                return SearchList.ElementAtOrDefault(SelectedItemIndex);
        }
    }
}