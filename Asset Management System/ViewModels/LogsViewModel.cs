using Asset_Management_System.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows.Input;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Views;

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
            SearchCommand = new ViewModels.Base.RelayCommand(() => Search());
            PrintCommand = new Base.RelayCommand(() => Print());
        }

        

        #region Commands
        
        public ICommand SearchCommand { get; set; }
        public ICommand PrintCommand { get; set; }

        #endregion

        #region Public Properties
        public string SearchQueryText { get; set; } = "";
        public int SelectedItemIndex { get; set; }
        
        private ObservableCollection<Entry> _list = new ObservableCollection<Entry>();

        public ObservableCollection<Entry> SearchList
        {
            get => _list;
            set
            {
                _list.Clear();
                foreach (Entry entry in value)
                    _list.Add(entry);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sends a search request to the database, and sets the list of assets to the result.
        /// </summary>
        private void Search()
        {
            Console.WriteLine();
            Console.WriteLine("Searching for: " + SearchQueryText);
            LogRepository rep = new LogRepository();
            ObservableCollection<Entry> entries = new ObservableCollection<Entry>(rep.Search(SearchQueryText));

            Console.WriteLine("Found: " + entries.Count.ToString());
            
            /*
            if (entries.Count > 0)
                Console.WriteLine("-----------");

            //List<MenuItem> assetsFunc = new List<MenuItem>();
            /*
            foreach (Entry entry in entries)
            {
                Console.WriteLine(entry.LogableId);

                //// Creating menuItems
                //MenuItem item = new MenuItem();
                //MenuItem edit = new MenuItem() { Header = "Edit" };
                //MenuItem delete = new MenuItem() { Header = "Remove" };

                //item.Header = asset.Name;
                ////AddVisualChild(edit);
                //assetsFunc.Add(item);
            }
            /**/

            SearchList = entries;
        }

        /*
        private void Edit()
        {
            //Log selectedLog = GetSelectedItem();
            throw new NotImplementedException();
        }
        */
        /*
        private void Remove()
        {
            //Log selectedLog = GetSelectedItem();
            throw new NotImplementedException();
        }
        /**/
        private void Print()
        {
            // Copied from AssetViewModel
            var dialog = new PromtForReportName("log_report_" + DateTime.Now.ToString().Replace(@"/", "").Replace(@" ", "-").Replace(@":", "") + ".csv", "Report name:");
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
                        foreach (Entry entry in SearchList)
                        {
                            string fileEntry = entry.Id + "," + entry.LogableType.Name + " With id: " + entry.LogableId + ", Description: " + entry.Description + "Changes: " + entry.Options;
                            file.WriteLine(fileEntry);
                        }
                    }
                }
            }
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
