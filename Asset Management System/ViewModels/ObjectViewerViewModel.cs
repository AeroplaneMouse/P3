using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Views;
using System.Windows;
using System.Windows.Controls;
using Asset_Management_System.Logging;
using Asset_Management_System.Services.Interfaces;
using Asset_Management_System.ViewModels.ViewModelHelper;

namespace Asset_Management_System.ViewModels
{
    public class ObjectViewerViewModel : Base.BaseViewModel
    {
        #region Private Properties

        private MainViewModel _main;
        private ICommentService _commentService;
        private ICommentRepository _commentRep;

        private Tag TagInput;

        private Asset AssetInput;

        #endregion

        #region Public Properties

        public ObservableCollection<ShownField> FieldsList { get; set; }

        public ObservableCollection<Comment> CommentsList { get; set; }

        public List<Tag> TagsList { get; set; }

        public string Color { get; set; }

        public ulong ParentTag { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string CommentField { get; set; }

        public int SelectedItemIndex { get; set; }

        public bool IsTag { get; set; }

        #endregion

        #region Commands

        public ICommand AddNewCommentCommand { get; set; }

        public ICommand RemoveCommentCommand { get; set; }

        public ICommand ViewAssetHistoryCommand { get; set; }

        #endregion

        #region Constructor

        public ObjectViewerViewModel(MainViewModel main, ICommentService commentService, DoesContainFields inputObject)
        {
            // Reference to main view model
            _main = main;
            _commentService = commentService;
            _commentRep = _commentService.GetRepository() as ICommentRepository;

            FieldsList = new ObservableCollection<ShownField>();
            TagsList = new List<Tag>();
            CommentsList = new ObservableCollection<Comment>();

            // Initialize commands
            ViewAssetHistoryCommand = new Base.RelayCommand((ViewAssetHistory));
            AddNewCommentCommand = new Base.RelayCommand(AddNewComment);
            RemoveCommentCommand = new Base.RelayCommand(RemoveComment);


            // If the object being viewed is a tag
            if (inputObject is Tag tag)
            {
                TagInput = tag;

                Name = TagInput.Name;
                Color = TagInput.Color;
                ParentTag = TagInput.ParentID;
                IsTag = true;

                foreach (var field in TagInput.FieldsList)
                {
                    FieldsList.Add(new ShownField(field));
                }

                if (TagInput.ParentID != 0)
                {
                    TagRepository tagRepository = new TagRepository();
                    Tag parentTag = tagRepository.GetById(TagInput.ParentID);
                    foreach (var field in parentTag.FieldsList)
                    {
                        foreach (var shownField in FieldsList)
                        {
                            if (shownField.ShownFieldToFieldComparator(field))
                            {
                                shownField.FieldTags.Add(parentTag);
                            }
                        }
                    }
                }
            }

            // If the object being viewed is an asset
            else if (inputObject is Asset asset)
            {
                AssetInput = asset;

                Name = AssetInput.Name;
                Description = AssetInput.Description;
                IsTag = false;

                CommentsList = _commentRep.GetByAssetId(AssetInput.ID);

                foreach (var field in AssetInput.FieldsList)
                {
                    FieldsList.Add(new ShownField(field));
                }

                // Load all tags into TagsList
                LoadTags();

                // Get all the fields coming from the attached tags
                GetTagFields();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get the fields related to a tag, and add the relations to the FieldsList.
        /// </summary>
        private void GetTagFields()
        {
            if (TagsList.Count > 0)
            {
                foreach (var currentTag in TagsList)
                {
                    foreach (var field in currentTag.FieldsList)
                    {
                        foreach (var shownField in FieldsList)
                        {
                            if (shownField.ShownFieldToFieldComparator(field))
                            {
                                shownField.FieldTags.Add(currentTag);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Function to bind comment to a asset.
        /// </summary>
        private void AddNewComment()
        {
            if (!string.IsNullOrEmpty(CommentField))
            {
                Comment newComment = new Comment()
                {
                    Username = _main.CurrentUser,
                    Content = CommentField,
                    AssetID = AssetInput.ID
                };

                _commentRep.Insert(newComment, out ulong id);

                Log<Comment>.CreateLog(newComment, id);

                CommentField = string.Empty;

                CommentsList = _commentRep.GetByAssetId(AssetInput.ID);
            }
        }

        private void RemoveComment()
        {
            Comment selected = GetSelectedItem();

            if (selected != null)
            {
                Log<Comment>.CreateLog(selected, true);
                _commentRep.Delete(selected);
            }

            CommentsList = _commentRep.GetByAssetId(AssetInput.ID);
        }

        private Comment GetSelectedItem()
        {
            if (CommentsList.Count == 0)
            {
                return null;
            }

            return CommentsList.ElementAtOrDefault(SelectedItemIndex);
        }

        private void ViewAssetHistory()
        {
            new AssetHistory(AssetInput, _main.EntryService).ShowDialog();
        }

        /// <summary>
        /// Loads the tags attached to this asset
        /// </summary>
        private void LoadTags()
        {
            //TODO: Avoid doing this
            TagsList = ((IAssetRepository) AssetInput.GetRepository()).GetAssetTags(AssetInput);
            Console.WriteLine("Found " + TagsList.Count + " tags");
        }

        #endregion
    }
}