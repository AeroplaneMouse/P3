using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.ViewModels.Commands.ViewModelHelper;
using Asset_Management_System.Views;
using System.Windows;
using System.Windows.Controls;
using Asset_Management_System.Logging;

namespace Asset_Management_System.ViewModels
{
    public class ObjectViewerViewModel : Base.BaseViewModel
    {
        #region Private Properties

        private MainViewModel _main;

        private Tag TagInput;

        private Asset AssetInput;

        #endregion

        #region Public Properties

        public CommentRepository CommentRep { get; set; }

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

        public ObjectViewerViewModel(MainViewModel main, DoesContainFields inputObject)
        {
            // Reference to main view model
            _main = main;

            FieldsList = new ObservableCollection<ShownField>();
            TagsList = new List<Tag>();
            CommentsList = new ObservableCollection<Comment>();
            
            // Initialize commands
            ViewAssetHistoryCommand = new Base.RelayCommand((ViewAssetHistory));
            AddNewCommentCommand = new Base.RelayCommand(AddNewComment);
            RemoveCommentCommand = new Base.RelayCommand(RemoveComment);

            CommentRep = new CommentRepository();


            // If the object being viewed is a tag
            if (inputObject is Tag tag)
            {
                TagInput = tag;

                Name = TagInput.Name;
                Color = TagInput.Color;
                ParentTag = TagInput.ParentID;
                IsTag = true;

                TagInput.DeserializeFields();

                foreach (var field in TagInput.FieldsList)
                {
                    ShownField showField = new ShownField();
                    showField.Name = field.GetHashCode().ToString();
                    showField.Field = field;
                    FieldsList.Add(showField);
                }

                if (TagInput.ParentID != 0)
                {
                    //TODO G�r det her
                }
            }

            // If the object being view is an asset
            else if (inputObject is Asset asset)
            {
                AssetInput = asset;

                Name = AssetInput.Name;
                Description = AssetInput.Description;
                IsTag = false;

                AssetInput.DeserializeFields();

                CommentsList = CommentRep.GetByAssetId(AssetInput.ID);

                foreach (var field in AssetInput.FieldsList)
                {
                    ShownField shownField = new ShownField();
                    shownField.Name = field.GetHashCode().ToString();
                    shownField.Field = field;
                    FieldsList.Add(shownField);
                }

                // Load all tags into TagsList
                LoadTags();

                // Get all the fields coming from the attached tags
                GetTagFields();  
            }
        }

        #endregion

        #region Private Methods

        private void GetTagFields()
        {
            if (TagsList.Count > 0)
            {
                int fieldsCount = 0;
                foreach (var currentTag in TagsList)
                {
                    foreach (var field in currentTag.FieldsList)
                    {
                        foreach (var shownField in FieldsList)
                        {
                            if (Equals(field, shownField.Field))
                            {
                                shownField.FieldTags.Add(currentTag);
                            }
                        }
                    }
                }

                Console.WriteLine("Tags add " + fieldsCount + " fields");
            }
        }

        private void AddNewComment()
        {
            if (CommentField != null && CommentField != String.Empty)
            {
                Comment newComment = new Comment()
                {
                    Username = _main.CurrentUser,
                    Content = CommentField,
                    AssetID = AssetInput.ID,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                CommentRep.Insert(newComment);

                Log<Comment>.CreateLog(newComment);

                CommentField = String.Empty;

                CommentsList = CommentRep.GetByAssetId(AssetInput.ID);
            }
        }

        private void RemoveComment()
        {
            Comment selected = GetSelectedItem();

            if (selected != null)
            {
                Log<Comment>.CreateLog(selected, true);
                CommentRep.Delete(selected);
            }

            CommentsList = CommentRep.GetByAssetId(AssetInput.ID);
        }

        private Comment GetSelectedItem()
        {
            if (CommentsList.Count == 0)
                return null;

            else
            {
                return CommentsList.ElementAt(SelectedItemIndex);
            }
        }

        private void ViewAssetHistory()
        {
            var assetHistory = new AssetHistory(AssetInput);
            assetHistory.ShowDialog();
        }

        /// <summary>
        /// Loads the tags attached to this asset
        /// </summary>
        private void LoadTags()
        {
            AssetRepository rep = new AssetRepository();
            TagsList = rep.GetAssetTags(AssetInput);
            foreach (var tag in TagsList)
            {
                tag.DeserializeFields();
            }

            Console.WriteLine("Found " + TagsList.Count + " tags");
        }

        #endregion
    }

}
