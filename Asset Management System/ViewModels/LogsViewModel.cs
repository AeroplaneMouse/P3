using Asset_Management_System.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels
{
    public class LogsViewModel : Base.BaseViewModel
    {
        private MainViewModel _main;

        public LogsViewModel(MainViewModel main)
        {
            // Saving reference to the main window
            _main = main;
            //Search();
            
            // Initializing commands
            //AddNewCommand = new ViewModels.Base.RelayCommand(() => _main.ChangeMainContent(new Views.AssetManager(_main)));
            SearchCommand = new ViewModels.Base.RelayCommand(() => Search());
            EditCommand = new ViewModels.Base.RelayCommand(() => Edit());
            RemoveCommand = new ViewModels.Base.RelayCommand(() => Remove());
            PrintCommand = new Base.RelayCommand(() => Print());
        }

        

        #region Commands

        public ICommand AddNewCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand PrintCommand { get; set; }

        #endregion

        public string SearchQueryText { get; set; } = "";
        public int SelectedItemIndex { get; set; }

        //private ObservableCollection<Log> _list = new ObservableCollection<Log>();

        //public ObservableCollection<Log> SearchList
        //{
        //    get => _list;
        //    set
        //    {
        //        _list.Clear();
        //        foreach (Log log in value)
        //            _list.Add(log);
        //    }
        //}

        #region Private Methods

        private void Search()
        {
            //Log selectedLog = GetSelectedItem();
            throw new NotImplementedException();
        }

        private void Edit()
        {
            //Log selectedLog = GetSelectedItem();
            throw new NotImplementedException();
        }

        private void Remove()
        {
            //Log selectedLog = GetSelectedItem();
            throw new NotImplementedException();
        }

        private void Print()
        {
            //Log selectedLog = GetSelectedItem();
            throw new NotImplementedException();
        }

        //private Log GetSelectedItem()
        //{
        //    if (SearchList.Count == 0)
        //        return null;
        //    else
        //        return SearchList.ElementAt(SelectedItemIndex);
        //}

        #endregion
    }
}
