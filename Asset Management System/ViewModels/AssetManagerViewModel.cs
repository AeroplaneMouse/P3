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
using Asset_Management_System.Services.Interfaces;
using Asset_Management_System.ViewModels.Controllers;
using Asset_Management_System.ViewModels.Interfaces;

namespace Asset_Management_System.ViewModels
{
    public class AssetManagerViewModel : ObjectViewModelHelper, IAssetManager
    {
        private MainViewModel _main;
        
        private readonly Asset _asset;

        public ObservableCollection<ITagable> CurrentlyAddedTags;



        private bool Editing;

        public string Name { get => _asset.Name;
            set => _asset.Name = _asset.Name;
        }

        public string Identifier
        {
            get => _asset.Identifier;
            set => _asset.Identifier = _asset.Identifier;
        }

        public string Description
        {
            get => _asset.Description;
            set => _asset.Description = value;
        }

        public string Title { get; set; }

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

        private IAssetRepository _assetRep { get; set; }

        private IAssetService _service;

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


        public ICommand EnterKeyCommand { get; set; }

        public ICommand TabKeyCommand { get; set; }

        public ICommand SpaceKeyCommand { get; set; }

        public ICommand EscapeKeyCommand { get; set; }

        public ICommand BackspaceKeyCommand { get; set; }


        public AssetManagerViewModel(MainViewModel main, Asset inputAsset, IAssetService service, TextBox box, bool addMultiple = false)
        {
            _main = main;
            Title = "Edit asset";
            _service = service;
            _assetRep = _service.GetRepository() as IAssetRepository;

            if (inputAsset != null)
            {
                // Notify view about changes
                _asset = inputAsset;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Description));
                CurrentlyAddedTags = new ObservableCollection<ITagable>(_assetRep.GetTags(_asset));
                FieldsList = _objectManagerController.ShownFieldsInitializer(_asset.FieldsList,false);
                HiddenFields = _objectManagerController.ShownFieldsInitializer(_asset.FieldsList,true);
                Editing = true;
                if (!addMultiple)
                {
                    Editing = true;
                    // DO more
                }
            }
            else
            {
                CurrentlyAddedTags = new ObservableCollection<ITagable>();
                _asset = new Asset();
                Editing = false;
            }
            
            // Initialize commands
            SaveAssetCommand = new SaveAssetCommand( _main, _asset, _service,CurrentlyAddedTags, Editing);
            SaveMultipleAssetsCommand = new SaveAssetCommand( _main, _asset, _service, CurrentlyAddedTags,false, true);
            
            CancelCommand = new Base.RelayCommand(() => _main.ReturnToPreviousPage());

            #region Tag related variables

            // TODO: get via Dependency Injection
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

            UntagTagCommand = new RemoveRelationToTagCommand(this);
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
        /// This function runs upon selecting a Tag with Enter.
        /// </summary>
        private void Apply()
        {
            // If no tag matching searchString is found in _tagList
            if (_tagList.SingleOrDefault(tag => tag.Name == _searchString) == null)
            {
                _searchString = _tagList
                    .Select(tag => tag.Name)
                    .ElementAtOrDefault(0);
            }

            if (CurrentlyAddedTags.FirstOrDefault(p => Equals(p.TagLabel, _searchString)) == null)
            {
                Tag tag = _tagList.SingleOrDefault(p =>
                    String.Equals(p.Name, _searchString, StringComparison.CurrentCultureIgnoreCase));
                if (tag != null)
                {
                    CurrentlyAddedTags.Add(tag);
                }
                else
                {
                    _main.AddNotification(new Notification("No matching tags found", Notification.WARNING));
                }

                _objectManagerController.ConnectTags(CurrentlyAddedTags,FieldsList,HiddenFields);
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
            if (_parentID != 0) return;

            // Checks if the tag we are "going into" has any children
            ulong tempId = _tagList
                .Select(p => p.ID)
                .ElementAtOrDefault(_tabIndex == 0 ? 0 : _tabIndex - 1);
            List<Tag> childList = _tagRep.GetChildTags(tempId).ToList();

            // If the tag we are "going into" has children, we go into it
            if (childList?.Count != 0)
            {
                _parentString = _tagList
                    .Select(p => p.Name)
                    .ElementAtOrDefault(_tabIndex == 0 ? 0 : _tabIndex - 1);
                _searchString = String.Empty;
                _parentID = tempId;
                _tagList = childList;

                _tabIndex = 0;
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

        public bool AttachTag(Asset asset, ITagable tag)
        {
            return true;
        }

        public bool DetachTag(Asset asset, ITagable tag)
        {
            throw new NotImplementedException();
        }

        public List<ShownField> GetRelations(Asset asset, List<Tag> tags)
        {
            throw new NotImplementedException();
        }

        public bool RemoveRelations(Asset asset, Tag tag, List<ShownField> shownFields)
        {
            throw new NotImplementedException();
        }

        public Tag CreateTag(string name)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            throw new NotImplementedException();
        }

        public bool Remove()
        {
            throw new NotImplementedException();
        }

        public bool Update()
        {
            throw new NotImplementedException();
        }
    }
}