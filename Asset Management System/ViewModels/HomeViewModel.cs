using Asset_Management_System.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels
{
    class HomeViewModel : Base.BaseViewModel
    {
        #region Private Properties

        private MainViewModel _main;

        // The string that the user is searching with
        private string _tagString { get; set; }

        // The id of the parent currently being used
        private ulong _parentID { get; set; } = 0;

        // List of all available tags, based on the search
        private List<Models.Tag> _tagList { get; set; }

        private ObservableCollection<Models.Tag> _2tagList { get; set; }

        // Index that the user has tabbed to in the search results
        private int _tabIndex { get; set; } = 0;

        // The name of the parent currently displayed
        private string _parentString { get; set; }

        // A tag repository, for communication with the database
        private Database.Repositories.TagRepository _rep { get; set; }

        // TODO: Kom uden om mig
        private TextBox _box { get; set; }

        #endregion


        #region Public Properties

        public string NumberOfAssets { get; set; }
        public string NumberOfTags { get; set; }
        public string NumberOfDepartments { get; set; }

        // The current parent exposed to the view
        public string ParentID
        {
            get
            {
                if (_parentID == 0)
                {
                    return "Tag groups:";
                }

                return _rep.GetById(_parentID).Name + ":";
            }
        }

        public List<string> StringList = new List<string>()
        {
            "Hej",
            "Noget",
            "Hejsa"
        };

        // The list exposed to the View
        public List<Models.Tag> TagList
        {
            get
            {
                if (_tagList == null)
                {
                    _tagList = _rep.GetChildTags(_parentID) as List<Models.Tag>;
                }

                return _tagList as List<Models.Tag>;
            }

            set { }
        }

        // The string that is being searched for, exposed to the view
        public string TagString
        {
            get
            {
                return _tagString;
            }

            set
            {
                SearchAndSortTagList(value);

                _tabIndex = 0;

                _tagString = value;
            }
        }

        #endregion


        #region Commands

        public ICommand EnterKeyCommand { get; set; }

        public ICommand TabKeyCommand { get; set; }

        public ICommand SpaceKeyCommand { get; set; }

        public ICommand EscapeKeyCommand { get; set; }

        public ICommand BackspaceKeyCommand { get; set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Default contructor
        /// </summary>
        public HomeViewModel(MainViewModel main, TextBox box)
        {
            _main = main;

            _rep = new TagRepository();

            // TODO: Kom uden om mig
            _box = box;

            // When the enter key is pressed, do something!
            EnterKeyCommand = new Base.RelayCommand(() => Apply());

            // Tabs through the search results
            TabKeyCommand = new Base.RelayCommand(() => CycleResults());

            // "Goes into" a parent tags children, and limits the search to these
            SpaceKeyCommand = new Base.RelayCommand(() => EnterChildren());

            // Start the search over
            EscapeKeyCommand = new Base.RelayCommand(() => ResetSearch());

            // Deletes charaters from the search query, and if the query is empty, go up a tag level
            BackspaceKeyCommand = new Base.RelayCommand(() => DeleteCharacter());

            // Get the number of stored assets, tags and departments
            int assets = new AssetRepository().GetCount();
            int tags = new TagRepository().GetCount();
            int departments = new DepartmentRepository().GetCount();

            // Generate strings
            NumberOfAssets = $"You have { assets } asets";
            NumberOfTags = $"You have { tags } tags";
            NumberOfDepartments = $"You have { departments } departments";

            // Notify view
            OnPropertyChanged(nameof(NumberOfAssets));
            OnPropertyChanged(nameof(NumberOfTags));
            OnPropertyChanged(nameof(NumberOfDepartments));
        }

        #endregion


        #region private Methods

        /// <summary>
        /// Searches through the list of tags, for all the tags that contains the search query,
        /// and sorts them by looking at what each tag label starts with
        /// </summary>
        /// <param name="value">The given search query</param>
        private void SearchAndSortTagList(string value)
        {
            _tagList = _rep.GetChildTags(_parentID)
                .Where(p => p.Name.ToLower().Contains(value.ToLower()))
                .OrderByDescending(p => p.Name.ToLower().StartsWith(value.ToLower()))
                .ToList();

            foreach (var item in _tagList)
            {
                Console.WriteLine(item.Name);
            }

            Console.WriteLine();
        }

        private void Apply()
        {
            MessageBox.Show("You searched for: " + _tagString);
            _tabIndex = 0;
        }

        private void CycleResults()
        {


            _tagString = _tagList.Select(p => p.Name).ElementAtOrDefault(_tabIndex++);

            if (_tagString == null)
            {
                _tabIndex = 0;
                _tagString = _tagList.Select(p => p.Name).ElementAtOrDefault(_tabIndex);
            }

            // TODO: Kom uden om mig
            _box.CaretIndex = _tagString.Length;

        }

        private void EnterChildren()
        {
            // Can only go in, if the parent tag is at the highest level
            if (_parentID == 0)
            {
                // Checks if the tag we are "going into" has any children
                ulong tempID = _tagList.Select(p => p.ID).ElementAtOrDefault(_tabIndex == 0 ? 0 : _tabIndex - 1);
                List<Models.Tag> tempList = _rep.GetChildTags(tempID) as List<Models.Tag>;

                // If the tag we are "going into" has children, we go into it
                if (tempList.Count() != 0)
                {
                    _parentString = _tagList.Select(p => p.Name).ElementAtOrDefault(_tabIndex == 0 ? 0 : _tabIndex - 1);
                    _tagString = String.Empty;
                    _parentID = tempID;
                    _tagList = tempList;

                    _tabIndex = 0;
                }
            }
        }

        private void ResetSearch()
        {
            _tagString = String.Empty;
            _parentID = 0;
            _tagList = _rep.GetChildTags(0) as List<Models.Tag>;
            _tabIndex = 0;
        }

        private void DeleteCharacter()
        {
            // If the search query is empty, the search goes up a level (to the highest level of tags)
            if (_tagString == String.Empty && _parentID != 0)
            {
                _parentID = 0;
                _tagList = _rep.GetChildTags(_parentID) as List<Models.Tag>;

                _tagString = _parentString;
                SearchAndSortTagList(_tagString);

                // TODO: Kom uden om mig
                _box.CaretIndex = _tagString.Length;
                return;
            }

            // If the search query isn't empty, a letter is simply removed
            else if (_tagString != String.Empty && _tagString != null)
            {
                _tagString = _tagString.Substring(0, _tagString.Length - 1);

                SearchAndSortTagList(_tagString);

                // TODO: Kom uden om mig
                _box.CaretIndex = _tagString.Length;
            }
        }

        #endregion




    }
}
