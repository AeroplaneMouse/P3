using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using AMS.Controllers.Interfaces;
using AMS.Events;
using AMS.Helpers;
using AMS.Interfaces;
using AMS.Models;
using AMS.ViewModels.Base;
using AMS.Views.Prompts;

namespace AMS.ViewModels
{
    public class AssetEditorViewModel : BaseViewModel
    {
        #region Public Properties

        public ObservableCollection<Field> NonHiddenFieldList =>
            new ObservableCollection<Field>(_assetController.NonHiddenFieldList);

        public ObservableCollection<Field> HiddenFieldList =>
            new ObservableCollection<Field>(_assetController.HiddenFieldList);

        // TODO: Skal fjernes?
        //public List<Tag> TagList { get; set; }

        public ObservableCollection<ITagable> AppliedTags { get; set; } = new ObservableCollection<ITagable>();

        public ObservableCollection<ITagable> TagSearchSuggestions { get; set; }

        public string Name
        {
            get => _assetController.Name;
            set => _assetController.Name = value;
        }

        public string Identifier
        {
            get => _assetController.Identifier;
            set => _assetController.Identifier = value;
        }

        public string Description
        {
            get => _assetController.Description;
            set => _assetController.Description = value;
        }

        public string Title { get; set; }

        public string TagSearchQuery
        {
            get => _tagSearchQuery;
            set
            {
                _tagSearchQuery = value;
                TagSearch();
            }
        }
        public string CurrentGroup { get; set; }
        public Visibility CurrentGroupVisibility { get; set; } = Visibility.Collapsed;
        public Visibility TagSuggestionsVisibility { get; set; } = Visibility.Collapsed;
        public Visibility SingleSelected { get; set; } = Visibility.Collapsed;
        public Visibility MultipleSelected { get; set; } = Visibility.Collapsed;
        public bool TagSuggestionIsOpen { get; set; } = false;
        public ITagable TagParent { get; set; }

        #endregion

        #region Private Properties

        private IAssetController _assetController { get; set; }
        private bool _isEditing { get; set; }
        private string _tagSearchQuery { get; set; }
        private TagHelper _tagHelper { get; set; }

        #endregion

        #region Comands

        public ICommand AddFieldCommand { get; set; }
        public ICommand RemoveFieldCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand SaveMultipleCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand RemoveTagCommand { get; set; }
        public ICommand AddTagCommand { get; set; }
        public ICommand AutoTagCommand { get; set; }
        public ICommand ClearInputCommand { get; set; }
        public ICommand EnterSuggestionListCommand { get; set; }

        #endregion

        #region Constructor

        public AssetEditorViewModel(IAssetController assetController, TagHelper tagHelper)
        {
            _assetController = assetController;

            _tagHelper = tagHelper;
            _tagHelper.CanApplyParentTags = false;
            _tagHelper.SetCurrentTags(new ObservableCollection<ITagable>(_assetController.CurrentlyAddedTags));
            AppliedTags = _tagHelper.GetAppliedTags(false);

            _isEditing = (_assetController.ControlledAsset.ID != 0);


            Title = _isEditing ? "Edit asset" : "Add asset";

            // Commands
            SaveCommand = new RelayCommand(() => SaveAndExit());
            SaveMultipleCommand = new RelayCommand(() => SaveAsset());
            AddFieldCommand = new RelayCommand(() => PromptForCustomField());
            CancelCommand = new RelayCommand(() => Cancel());
            RemoveFieldCommand = new RelayCommand<object>((parameter) => RemoveField(parameter));
            AddTagCommand = new RelayCommand(() => TagSearch());

            RemoveTagCommand = new RelayCommand<object>((parameter) =>
            {
                ITagable tag = parameter as ITagable;
                _tagHelper.RemoveTag(tag);
                _assetController.DetachTag(tag);
                AppliedTags = _tagHelper.GetAppliedTags(false);
                UpdateAll();
            });

            AutoTagCommand = new RelayCommand(() => AutoTag());
            ClearInputCommand = new RelayCommand(ClearInput);
            UpdateAll();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Saves and leaves the page
        /// </summary>
        public void SaveAndExit()
        {
            if (!VerifyAssetAndFields())
                return;

            SaveAsset(false);

            Features.Navigate.Back();
        }

        /// <summary>
        /// Saves the asset.
        /// </summary>
        /// <param name="multiAdd"></param>
        public void SaveAsset(bool multiAdd = true)
        {

            //Checks whether to save a new, or update an existing.
            if (_isEditing)
            {
                if (!multiAdd)
                {
                    _assetController.Update();
                    Features.AddNotification(new Notification("Asset updated", Notification.APPROVE));
                }
                else
                {
                    _assetController.Save();
                    Features.AddNotification(new Notification("Asset added", Notification.APPROVE));
                }
            }
            else
            {
                _assetController.Save();
                Features.AddNotification(new Notification("Asset added", Notification.APPROVE));
            }
        }

        /// <summary>
        /// Prompt for custom fields.
        /// </summary>
        public void PromptForCustomField()
        {
            Features.DisplayPrompt(new CustomField("Add field", AddCustomField, true));
        }

        /// <summary>
        /// Adds a custom fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AddCustomField(object sender, PromptEventArgs e)
        {
            if (e is FieldInputPromptEventArgs args)
            {
                _assetController.AddField(args.Field);
                UpdateAll();
            }
        }

        /// <summary>
        /// The command for removing a field.
        /// </summary>
        /// <param name="sender"></param>
        public void RemoveField(object sender)
        {
            if (sender is Field field)
            {
                _assetController.RemoveField(field);
                UpdateAll();
            }
        }

        /// <summary>
        /// Runs the cancel command, and returns.
        /// </summary>
        public void Cancel()
        {
            if (Features.Navigate.Back() == false)
            {
                Features.Navigate.To(Features.Create.AssetList());
            }
        }

        /// <summary>
        /// Attach given tag to the asset
        /// </summary>
        /// <param name="tag"></param>
        public void AutoTag()
        {

            if (TagSearchSuggestions != null && TagSearchSuggestions.Count > 0)
            {
                ITagable tag = TagSearchSuggestions[0];

                if (_tagHelper.IsParentSet() || (tag.ChildrenCount == 0 && tag.TagId != 1))
                {
                    _tagHelper.ApplyTag(tag);
                    _assetController.AttachTag(tag);
                    AppliedTags = _tagHelper.GetAppliedTags(false);
                    TagSearchQuery = "";
                    UpdateAll();
                    //TagSearchProcess();
                }
                else
                {
                    // So we need to switch to a group of tags.
                    Tag taggedItem = (Tag)tag;
                    _tagHelper.Parent(taggedItem);
                    CurrentGroup = taggedItem.Name;
                    CurrentGroupVisibility = Visibility.Visible;
                    TagSearchQuery = "";
                }
            }

            TagSearchProcess();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Runs the tagsearch process.
        /// </summary>
        private void TagSearch()
        {
            TagSearchProcess();
        }

        /// <summary>
        /// Clears the input in the searchbar
        /// </summary>
        private void ClearInput()
        {
            if (_tagHelper.IsParentSet())
            {
                _tagHelper.Parent(null);
                CurrentGroup = "";
                CurrentGroupVisibility = Visibility.Collapsed;
                TagSearchQuery = "";
                TagSearchProcess();
            }
        }

        /// <summary>
        /// Runs the tag process
        /// </summary>
        private void TagSearchProcess()
        {
            TagSearchSuggestions = new ObservableCollection<ITagable>(_tagHelper.Suggest(_tagSearchQuery));

            //Checks the suggestions
            if (_tagHelper.HasSuggestions())
            {
                TagSuggestionsVisibility = Visibility.Visible;
                TagSuggestionIsOpen = true;
            }
            else
            {
                TagSuggestionsVisibility = Visibility.Collapsed;
                TagSuggestionIsOpen = false;
            }
        }
        
        /// <summary>
        /// Updates the elements of the view.
        /// </summary>
        private void UpdateAll()
        {
            UpdateTagRelations(AppliedTags);
            OnPropertyChanged(nameof(AppliedTags));
            OnPropertyChanged(nameof(NonHiddenFieldList));
            OnPropertyChanged(nameof(HiddenFieldList));
        }

        /// <summary>
        /// Updates the relations from the tag, and adds fields.
        /// </summary>
        /// <param name="tagsList"></param>
        private void UpdateTagRelations(ObservableCollection<ITagable> tagsList)
        {
            // Runs through the list of tagID's, and adds the tag with the same tagID to the TagList on the field.
            foreach (var field in HiddenFieldList)
            {
                field.TagList = new List<Tag>();
                foreach (var id in field.TagIDs)
                {
                    if (tagsList.SingleOrDefault(p => p.TagId == id) is Tag tag)
                    {
                        field.TagList.Add(tag);
                    }
                }
            }

            foreach (var field in NonHiddenFieldList)
            {
                field.TagList = new List<Tag>();
                foreach (var id in field.TagIDs)
                {
                    if (tagsList.SingleOrDefault(p => p.TagId == id) is Tag tag)
                    {
                        field.TagList.Add(tag);
                    }
                }
            }
        }

        /// <summary>
        /// Verifies the fields, and the asset, checks if the required elements have been completed.
        /// </summary>
        /// <returns></returns>
        private bool VerifyAssetAndFields()
        {
            if (string.IsNullOrEmpty(Name))
            {
                Features.AddNotification(new Notification("The field " + "Name" + " is required and empty",Notification.WARNING));
                return false;
            }
            
            //Verifies whether fields contains correct information, or the required information.
            List<Field> completeList = HiddenFieldList.ToList();
            completeList.AddRange(NonHiddenFieldList.ToList());
            
            foreach (var field in completeList)
            {
                if (field.Required && string.IsNullOrEmpty(field.Content))
                {
                    Features.AddNotification(new Notification("The field " + field.Label + " is required and empty",Notification.WARNING));
                    return false;
                }

                if (field.Type == Field.FieldType.NumberField)
                {
                    bool check = field.Content.All(char.IsDigit);
                    if (check)
                    {
                        Features.AddNotification(
                            new Notification("The field " + field.Label + " cannot contain letters",Notification.WARNING));
                        return false;
                    }
                }

                if (field.Type == Field.FieldType.Date && string.Equals(field.Content,"today"))
                {
                    field.Content = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                }
            }

            return true;
        }

        #endregion
    }
}