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
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Services.Interfaces;
using Asset_Management_System.ViewModels.Base;

namespace Asset_Management_System.ViewModels
{
    public abstract class ListPageViewModel<T> : Base.BaseViewModel, IListUpdate
        where T : class, new()
    {
        protected MainViewModel _main;
        private IDisplayableService<T> _service;
        private ICommentService _commentService;
        private ObservableCollection<T> _list { get; set; }
        private string _searchQueryText { get; set; }
        
        protected ISearchableRepository<T> Rep { get; private set; }
        public Visibility Visible
        {
            get => _main.Visible;
            private set => Visible = value;
        }
        public List<DoesContainFields> SelectedItems { get; set; } = new List<DoesContainFields>();
        public bool MultipleSelected {
            get
            {
               return SelectedItems.Count > 0;
            }
            set => MultipleSelected = value;
        }
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
                    _list.Add(item);

                OnPropertyChanged(nameof(SearchList));
            }
        }

        // Commands
        public ICommand PrintCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ViewCommand { get; set; }
        public ICommand HeaderClickCommand { get; set; }
        public ICommand RemoveCommand { get; set; }

        public ListPageViewModel(MainViewModel main, IDisplayableService<T> service, ICommentService commentService = default)
        {
            _main = main;
            _service = service;
            _commentService = commentService;
            Rep = _service.GetSearchableRepository();

            _searchQueryText = String.Empty;
            _list = new ObservableCollection<T>();

            // Initialize commands
            PrintCommand = new Commands.PrintSelectedItemsCommand(_main);
            SearchCommand = new Base.RelayCommand(Search);
            ViewCommand = new Base.RelayCommand(View);
            HeaderClickCommand = new Base.RelayCommand<object>(HeaderClick);
            RemoveCommand = new RelayCommand(RemoveItems);
        }


        public virtual void PageGotFocus()
        {
            Search();
        }

        public virtual void PageLostFocus()
        {

        }

        private void RemoveItems()
        {
            /*
            foreach (var item in SelectedItems)
            {

            }
            */
        }

        /// <summary>
        /// Sends a search request to the database, and sets the list of items to the result.
        /// </summary>
        public virtual void Search() => SearchList = Rep.Search(SearchQueryText);

        /// <summary>
        /// Creates a csv file containing all the items
        /// </summary>
        protected void Print()
        {
            if (SearchList != null && SearchList.Count > 0)
                PrintHelper.Print(SearchList as IEnumerable<object>);
            else
                _main.AddNotification(new Notification("Error! Cannot export an empty list.", Notification.ERROR));
        }


        /// <summary>
        /// Displays the selected item
        /// </summary>
        protected virtual void View()
        {
            T selected = GetSelectedItem();

            switch (selected)
            {
                case null:
                    return;
                case Entry entry:
                    new ShowEntry(entry).ShowDialog();
                    break;
                case DoesContainFields fields:
                    _main.ChangeMainContent(new ObjectViewer(_main, _commentService, fields));
                    break;
                default:
                    _main.AddNotification(
                        new Notification("Error! Unknown error occured when trying to view.", Notification.ERROR),
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
                            .OrderBy(i => i.Name).Cast<T>()),
                        "ID" => new ObservableCollection<T>(SearchList.Cast<Asset>()
                            .OrderBy(i => i.ID).Cast<T>()),
                        "Identifier" => new ObservableCollection<T>(SearchList.Cast<Asset>()
                            .OrderBy(i => i.Identifier).Cast<T>()),
                        "Created" => new ObservableCollection<T>(SearchList.Cast<Asset>()
                            .OrderBy(i => i.CreatedAt).Cast<T>()),
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
                            .OrderBy(i => i.Name).Cast<T>()),
                        "ID" => new ObservableCollection<T>(SearchList.Cast<Tag>()
                            .OrderBy(i => i.ID).Cast<T>()),
                        "Parent Tag ID" => new ObservableCollection<T>(SearchList.Cast<Tag>()
                            .OrderBy(i => i.ParentID).Cast<T>()),
                        "Department ID" => new ObservableCollection<T>(SearchList.Cast<Tag>()
                            .OrderBy(i => i.DepartmentID).Cast<T>()),
                        "Color" => new ObservableCollection<T>(SearchList.Cast<Tag>()
                            .OrderBy(i => i.Color).Cast<T>()),
                        "Created" => new ObservableCollection<T>(SearchList.Cast<Tag>()
                            .OrderBy(i => i.CreatedAt).Cast<T>()),
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
                            .OrderBy(i => i.CreatedAt).Cast<T>()),
                        "ID" => new ObservableCollection<T>(SearchList.Cast<Entry>()
                            .OrderBy(i => i.ID).Cast<T>()),
                        "Event" => new ObservableCollection<T>(SearchList.Cast<Entry>()
                            .OrderBy(i => i.Description).Cast<T>()),
                        "User" => new ObservableCollection<T>(SearchList.Cast<Entry>()
                            .OrderBy(i => i.Username).Cast<T>()),
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
