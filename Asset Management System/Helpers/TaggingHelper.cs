using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Services.Interfaces;
using Asset_Management_System.ViewModels;

namespace Asset_Management_System.Helpers
{
    public class TaggingHelper : ObjectManagerController
    {
        //TODO Complete this class later, cuz it's a lot of work

        private List<Tag> _tagList;
        private Asset _asset;
        private ulong _parentID;
        private ITagRepository _tagRep;
        private IAssetService _assetService;
        private IAssetRepository _assetRep;
        private string _searchString;
        private MainViewModel _main;
        public ObservableCollection<Tag> CurrentlyAddedTags;

        public TaggingHelper(MainViewModel main, Asset asset, IAssetService assetService, ITagRepository tagRep)
        {
            _main = main;
            _assetService = assetService;
            _assetRep = _assetService.GetRepository() as IAssetRepository;
            _tagRep = tagRep;
            _asset = asset;
            CurrentlyAddedTags = new ObservableCollection<Tag>(_assetRep.GetAssetTags(_asset));

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
        
        #endregion
        
    }
}