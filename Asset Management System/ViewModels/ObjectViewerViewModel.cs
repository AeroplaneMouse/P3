using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Views;

namespace Asset_Management_System.ViewModels
{
    public class ObjectViewerViewModel
    {
        private Tag TagInput;

        private Asset AssetInput;

        public string Color { get; set; }

        public ulong ParentTag { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsTag { get; set; }

        public bool HasComment { get; set; }

        public ICommand AddNewCommentCommand { get; set; }

        public CommentRepository CommentRep { get; set; }
        


        public ICommand ViewAssetCommand { get; set; }

        public ObservableCollection<ShowFields> FieldsList { get; set; }

        public ObservableCollection<Comment> CommentList { get; set; }

        public List<Tag> TagsList { get; set; }

        private MainViewModel _main;

        public ObjectViewerViewModel(MainViewModel main, DoesContainFields inputObject)
        {
            FieldsList = new ObservableCollection<ShowFields>();
            TagsList = new List<Tag>();
            _main = main;
            ViewAssetCommand = new Base.RelayCommand((ViewAssetHistory));

            CommentList = new ObservableCollection<Comment>();

            CommentRep = new CommentRepository();

            // AddNewCommentCommand

            if (inputObject is Tag tag)
            {
                TagInput = tag;
                tag.DeserializeFields();
                foreach (var field in tag.FieldsList)
                {
                    ShowFields showField = new ShowFields();
                    showField.Name = field.GetHashCode().ToString();
                    showField.Field = field;
                    FieldsList.Add(showField);
                }

                if (tag.ParentID != 0)
                {
                    
                }

                Name = tag.Name;
                Color = tag.Color;
                ParentTag = tag.ParentID;
                IsTag = true;
                HasComment = false;
            }
            else if (inputObject is Asset asset)
            {
                AssetInput = asset;
                asset.DeserializeFields();
                foreach (var field in asset.FieldsList)
                {
                    ShowFields showField = new ShowFields();
                    showField.Name = field.GetHashCode().ToString();
                    showField.Field = field;
                    FieldsList.Add(showField);
                }

                LoadTags();
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

                Name = asset.Name;
                Description = asset.Description;
                IsTag = false;

                CommentList = CommentRep.GetByAssetId(asset.ID);

                HasComment = CommentList.Count > 0;

                Console.WriteLine("Comments:");

                foreach (Comment comment in CommentList)
                {
                    Console.WriteLine(comment.Content);
                }

                Console.WriteLine();
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
    }

    public class ShowFields
    {
        public string Name { get; set; }

        public Field Field { get; set; }

        public ObservableCollection<Tag> FieldTags { get; set; }

        public ShowFields()
        {
            FieldTags = new ObservableCollection<Tag>();
        }
    }
}