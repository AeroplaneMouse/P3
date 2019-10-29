using Asset_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.ViewModels.Commands.ViewModelHelper;

namespace Asset_Management_System.ViewModels
{
    public class AssetManagerViewModel : FieldsController
    {
        private MainViewModel _main;
        private Asset _asset;

        public string Name { get; set; }
        public string Description { get; set; }

        #region Tag related private properties

        // The string that the user is searching with
        private string _searchString { get; set; }



        // The id of the parent currently being used
        private ulong _parentID { get; set; } = 0;

        // List of all available tags, based on the search
        private List<Tag> _tagList { get; set; }

        // Index that the user has tabbed to in the search results
        private int _tabIndex { get; set; } = 0;

        // The name of the parent currently displayed
        private string _parentString { get; set; }

        // A tag repository, for communication with the database
        private Database.Repositories.TagRepository _tagRep { get; set; }

        // TODO: Kom uden om mig
        private TextBox _box { get; set; }


        private List<Models.Asset> _assetList { get; set; }

        private Database.Repositories.AssetRepository _assetRep { get; set; }

        #endregion


        #region tag related public Properties

        public string NumberOfAssets { get; set; }
        public string NumberOfTags { get; set; }
        public string NumberOfDepartments { get; set; }
        
        public ObservableCollection<Tag> CurrentlyAddedTags { get; set; }

        public List<Models.Asset> AssetList;

        // The current parent exposed to the view
        public string ParentID
        {
            get
            {
                if (_parentID == 0)
                {
                    return "Tag groups:";
                }

                return _tagRep.GetById(_parentID).Name + ":";
            }
        }

        // The list exposed to the View
        public List<Tag> TagList
        {
            get
            {
                if (_tagList == null)
                {
                    _tagList = _tagRep.GetChildTags(_parentID) as List<Models.Tag>;
                }

                return _tagList.Take(10).ToList();
            }

            set { }
        }

        // The string that is being searched for, exposed to the view
        public string TagString
        {
            get { return _searchString; }

            set
            {
                SearchAndSortTagList(value);

                _tabIndex = 0;

                _searchString = value;
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


        public AssetManagerViewModel(MainViewModel main, Asset inputAsset, TextBox box)
        {
            _main = main;

            CurrentlyAddedTags = new ObservableCollection<Tag>();
            FieldsList = new ObservableCollection<ShownField>();
            if (inputAsset != null)
            {
                _asset = inputAsset;
                LoadFields();
                _editing = true;
            }
            else
            {
                _asset = new Asset();
                _editing = false;
            }

            // Initialize commands
            SaveAssetCommand = new Commands.SaveAssetCommand(this, _main, _asset, _editing);
            AddFieldCommand = new Commands.AddFieldCommand(this);
            RemoveFieldCommand = new Commands.RemoveFieldCommand(this);

            #region Tag related variables

            _tagRep = new TagRepository();

            _assetRep = new Database.Repositories.AssetRepository();

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
            NumberOfAssets = $"You have {assets} assets";
            NumberOfTags = $"You have {tags} tags";
            NumberOfDepartments = $"You have {departments} departments";

            // Notify view
            OnPropertyChanged(nameof(NumberOfAssets));
            OnPropertyChanged(nameof(NumberOfTags));
            OnPropertyChanged(nameof(NumberOfDepartments));

            #endregion
        }

        public ICommand SaveAssetCommand { get; set; }
        public static ICommand RemoveFieldCommand { get; set; }


        public bool CanSaveAsset()
        {
            // **** TODO ****
            // Only return true, if the entered values are valid.
            return true;
        }


        protected override void LoadFields()
        {
            _asset.DeserializeFields();
            foreach (var field in _asset.FieldsList)
            {
                ShownField shownField = new ShownField();
                shownField.Name = field.GetHashCode().ToString();
                shownField.Field = field;
                FieldsList.Add(shownField);
            }

            Name = _asset.Name;
            Description = _asset.Description;

            // Notify view about changes
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Description));
        }

        #region Tag search related methods

        /// <summary>
        /// Searches through the list of tags, for all the tags that contains the search query,
        /// and sorts them by looking at what each tag label starts with
        /// </summary>
        /// <param name="value">The given search query</param>
        private void SearchAndSortTagList(string value)
        {
            _tagList = _tagRep.GetChildTags(_parentID)
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
            CurrentlyAddedTags.Add(_tagList.Single(p =>
                String.Equals(p.Name, _searchString, StringComparison.CurrentCultureIgnoreCase)));
            Console.WriteLine("Checking:  " + _tagList.Count);
            ConnectTags(CurrentlyAddedTags);
            
            _tabIndex = 0;
        }

        private void CycleResults()
        {
            _searchString = _tagList.Select(p => p.Name).ElementAtOrDefault(_tabIndex++);

            if (_searchString == null)
            {
                _tabIndex = 0;
                _searchString = _tagList.Select(p => p.Name).ElementAtOrDefault(_tabIndex);
            }

            // TODO: Kom uden om mig
            _box.CaretIndex = _searchString.Length;
        }

        private void EnterChildren()
        {
            // Can only go in, if the parent tag is at the highest level
            if (_parentID == 0)
            {
                // Checks if the tag we are "going into" has any children
                ulong tempID = _tagList.Select(p => p.ID).ElementAtOrDefault(_tabIndex == 0 ? 0 : _tabIndex - 1);
                List<Models.Tag> tempList = _tagRep.GetChildTags(tempID) as List<Models.Tag>;

                // If the tag we are "going into" has children, we go into it
                if (tempList.Count() != 0)
                {
                    _parentString = _tagList.Select(p => p.Name).ElementAtOrDefault(_tabIndex == 0 ? 0 : _tabIndex - 1);
                    _searchString = String.Empty;
                    _parentID = tempID;
                    _tagList = tempList;

                    _tabIndex = 0;
                }
            }
        }

        private void ResetSearch()
        {
            _searchString = String.Empty;
            _parentID = 0;
            _tagList = _tagRep.GetChildTags(0) as List<Models.Tag>;
            _tabIndex = 0;
        }

        private void DeleteCharacter()
        {
            // If the search query is empty, the search goes up a level (to the highest level of tags)
            if (_searchString == String.Empty && _parentID != 0)
            {
                _parentID = 0;
                _tagList = _tagRep.GetChildTags(_parentID) as List<Models.Tag>;

                _searchString = _parentString;
                SearchAndSortTagList(_searchString);

                // TODO: Kom uden om mig
                _box.CaretIndex = _searchString.Length;
                return;
            }

            // If the search query isn't empty, a letter is simply removed
            else if (_searchString != String.Empty && _searchString != null)
            {
                _searchString = _searchString.Substring(0, _searchString.Length - 1);

                SearchAndSortTagList(_searchString);

                // TODO: Kom uden om mig
                _box.CaretIndex = _searchString.Length;
            }
        }

        private void ConnectTags(ObservableCollection<Tag> tags)
        {
            foreach (var tag in tags)
            {
                
            }
        }

        #endregion
    }
}