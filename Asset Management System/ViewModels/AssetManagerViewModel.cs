using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Asset_Management_System.Models;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.ViewModels.Commands;
using Asset_Management_System.ViewModels.ViewModelHelper;
using System.Drawing;
using Asset_Management_System.Services;
using Asset_Management_System.Services.Interfaces;

namespace Asset_Management_System.ViewModels
{
    public class AssetManagerViewModel : ObjectManagerController
    {
        private MainViewModel _main;
        private Asset _asset;

        public string Name { get; set; }
        public string Identifier { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;

        public string Title { get; set; }

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
        private ITagRepository _tagRep { get; set; }

        // TODO: Kom uden om mig
        private TextBox _box { get; set; }


        private List<Asset> _assetList { get; set; }

        private IAssetRepository _assetRep { get; set; }

        private IAssetService _service;

        #endregion


        #region tag related public Properties

        public List<Asset> AssetList;

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
                    _tagList = _tagRep.GetChildTags(_parentID) as List<Tag>;
                }

                return _tagList.Take(10).ToList();
            }

            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
            }
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


        public AssetManagerViewModel(MainViewModel main, Asset inputAsset, IAssetService service, TextBox box, bool addMultiple = false)
        {
            _main = main;
            Title = "Edit asset";
            _service = service;
            
            _assetRep = _service.GetRepository() as IAssetRepository;

            FieldsList = new ObservableCollection<ShownField>();
            if (inputAsset != null)
            {
                _asset = inputAsset;
                LoadFields();

                CurrentlyAddedTags = new ObservableCollection<Tag>(_assetRep.GetAssetTags(_asset));

                ConnectTags();
                if (!addMultiple)
                {
                    _editing = true;
                }
                
            }
            else
            {
                CurrentlyAddedTags = new ObservableCollection<Tag>();
                _asset = new Asset();
                _editing = false;
            }

            // Initialize commands
            SaveAssetCommand = new SaveAssetCommand(this, _main, _asset, _service, _editing);
            SaveMultipleAssetsCommand = new SaveAssetCommand(this, _main, _asset, _service, false, true);
            AddFieldCommand = new AddFieldCommand(_main, this, true);
            RemoveFieldCommand = new RemoveFieldCommand(this);

            CancelCommand = new Base.RelayCommand(() => _main.ChangeMainContent(new Views.Assets(_main, _service)));

            //TODO: This whole tag related thing should be a class of its own
            #region Tag related variables

            _tagRep = new TagRepository();

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

            // Deletes characters from the search query, and if the query is empty, go up a tag level
            BackspaceKeyCommand = new Base.RelayCommand(() => DeleteCharacter());

            #endregion

            UntagTagCommand = new UntagTagCommand(this);
        }

        public ICommand SaveAssetCommand { get; set; }
        public ICommand SaveMultipleAssetsCommand { get; set; }
        public static ICommand UntagTagCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        /// <summary>
        /// Verification of the asset, before saving.
        /// </summary>
        /// <returns></returns>
        public bool CanSaveAsset()
        {
            //TODO Figure out the implementation for this one.
            return true;
        }

        /// <summary>
        /// This function loads the fields from the asset, and into the viewmodel.
        /// </summary>
        protected override void LoadFields()
        {
            foreach (var field in _asset.FieldsList)
            {
                if (field.IsHidden)
                {
                    HiddenFields.Add(new ShownField(field));
                }
                else
                {
                    FieldsList.Add(new ShownField(field));
                }
            }

            Name = _asset.Name;
            Identifier = _asset.Identifier;
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
        }

        /// <summary>
        /// This function runs uppon selecting a Tag with Enter.
        /// </summary>
        private void Apply()
        {
            if (_tagList.SingleOrDefault(tag => tag.Name == _searchString) == null)
            {
                _searchString = _tagList
                    .Select(tag => tag.Name)
                    .ElementAtOrDefault(0);
            }

            if (CurrentlyAddedTags.FirstOrDefault(p => Equals(p.Name, _searchString)) == null)
            {
                Tag tag = _tagList.SingleOrDefault(p =>
                    String.Equals(p.Name, _searchString, StringComparison.CurrentCultureIgnoreCase));
                if (tag != null)
                    CurrentlyAddedTags.Add(tag);
                else
                    _main.AddNotification(new Notification("No matching tags found", Notification.WARNING));
                ConnectTags();
            }

            foreach (var field in FieldsList)
            {
                Console.WriteLine(field.Field.Label);
                foreach (var tag in field.FieldTags)
                {
                    Console.WriteLine("    " + tag.Name);
                }
            }

            ResetSearch();

            _tabIndex = 0;
        }

        /// <summary>
        /// This function cycles the results within the dropdown of tags.
        /// </summary>
        private void CycleResults()
        {
            if (_tagList != null)
            {
                _searchString = _tagList
                    .Select(p => p.Name)
                    .ElementAtOrDefault(_tabIndex++);

                if (_searchString == null)
                {
                    _tabIndex = 0;
                    _searchString = _tagList
                        .Select(p => p.Name)
                        .ElementAtOrDefault(_tabIndex);
                }

                if (_searchString != null)
                {
                    // TODO: Kom uden om mig
                    _box.CaretIndex = _searchString.Length;
                }
            }
        }

        /// <summary>
        /// This function is used to navigate into
        /// </summary>
        private void EnterChildren()
        {
            // Can only go in, if the parent tag is at the highest level
            if (_parentID == 0)
            {
                // Checks if the tag we are "going into" has any children
                ulong tempID = _tagList
                    .Select(p => p.ID)
                    .ElementAtOrDefault(_tabIndex == 0 ? 0 : _tabIndex - 1);
                List<Tag> tempList = _tagRep.GetChildTags(tempID) as List<Tag>;

                // If the tag we are "going into" has children, we go into it
                if (tempList?.Count != 0)
                {
                    _parentString = _tagList
                        .Select(p => p.Name)
                        .ElementAtOrDefault(_tabIndex == 0 ? 0 : _tabIndex - 1);
                    _searchString = String.Empty;
                    _parentID = tempID;
                    _tagList = tempList;

                    _tabIndex = 0;
                }
            }
        }


        /// <summary>
        /// This function clears the searched list, as well as clears the input field.
        /// </summary>
        private void ResetSearch()
        {
            _searchString = String.Empty;
            _parentID = 0;
            _tagList = _tagRep.GetChildTags(0) as List<Tag>;
            _tabIndex = 0;
        }

        /// <summary>
        /// Deletes characters, or goes up a level in tags. (Goes to tags, where tag.parentID = 0;
        /// </summary>
        private void DeleteCharacter()
        {
            // If the search query is empty, the search goes up a level (to the highest level of tags)
            if (_searchString == String.Empty && _parentID != 0)
            {
                _parentID = 0;
                _tagList = _tagRep.GetChildTags(_parentID) as List<Tag>;

                _searchString = _parentString;
                SearchAndSortTagList(_searchString);

                // TODO: Kom uden om mig
                _box.CaretIndex = _searchString.Length;
                return;
            }

            // If the search query isn't empty, a letter is simply removed
            else if (!string.IsNullOrEmpty(_searchString))
            {
                _searchString = _searchString.Substring(0, _searchString.Length - 1);

                SearchAndSortTagList(_searchString);

                // TODO: Kom uden om mig
                _box.CaretIndex = _searchString.Length;
            }
        }

        #endregion
    }
}
