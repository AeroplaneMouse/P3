using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
                if(!_tagSearchQuery.EndsWith(' '))
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
        private int _tagTabIndex { get; set; }

        #endregion

        #region Comands

        public ICommand AddFieldCommand { get; set; }
        public ICommand RemoveFieldCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand SaveMultipleCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand RemoveTagCommand { get; set; }
        public ICommand ApplyTagOrEnterParentCommand { get; set; }
        public ICommand InsertNextOrSelectedSuggestionCommand { get; set; }
        public ICommand ClearInputCommand { get; set; }
        public ICommand EnterSuggestionListCommand { get; set; }
        public ICommand ShowFieldEditPromptCommand { get; set; }
        public ICommand RemoveCommand { get; set; }

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
            ApplyTagOrEnterParentCommand = new RelayCommand(() => ApplyTagOrEnterParent());

            RemoveTagCommand = new RelayCommand<object>((parameter) =>
            {
                ITagable tag = parameter as ITagable;
                _tagHelper.RemoveTag(tag);
                _assetController.DetachTag(tag);
                AppliedTags = _tagHelper.GetAppliedTags(false);
                UpdateAll();
            });

            ShowFieldEditPromptCommand = new RelayCommand<object>((parameter) =>

            {
                if (parameter is Field field && field.IsCustom)
                    Features.DisplayPrompt(new Views.Prompts.CustomField(null, EditFieldConfirmed, true, field));
                else
                    //TODO Handle not field event
                    return;
            });

            RemoveCommand = new RelayCommand(() =>
            {
                //TODO Handle remove command
            });

            InsertNextOrSelectedSuggestionCommand = new RelayCommand<object>((parameter) => InsertNextOrSelectedSuggestion(parameter));
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
            if (SaveAsset(false))
                Features.Navigate.Back();
        }

        /// <summary>
        /// Saves the asset.
        /// </summary>
        /// <param name="multiAdd"></param>
        public bool SaveAsset(bool multiAdd = true)
        {
            if (!VerifyAssetAndFields())
                return false;

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
            return true;
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
            if (e.Result && e is FieldInputPromptEventArgs args)
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
            if (!Features.Navigate.Back())
                Features.Navigate.To(Features.Create.AssetList());
        }

        /// <summary>
        /// Inserts the next suggestion into the tag search query, or the selected element from the list
        /// </summary>
        /// <param name="input">The selected element (optional)</param>
        private void InsertNextOrSelectedSuggestion(object input = null)
        {
            
            // If the input is not null, use the suggestion if possible
            if (input != null)
            {
                ITagable tag;

                if (input is TextBlock textBlock)
                {
                    tag = TagSearchSuggestions.SingleOrDefault(t => t.TagLabel == textBlock.Text);
                }
                else
                {
                    tag = (ITagable)input;
                }

                if (tag != null)
                {
                    if (_tagHelper.IsParentSet() || (tag.ChildrenCount == 0 && tag.TagId != 1))
                    {
                        _tagHelper.AddTag(tag);
                        _assetController.AttachTag(tag);
                    }
                    else
                    {
                        // So we need to switch to a group of tags.
                        Tag taggedItem = (Tag)tag;
                        _tagHelper.SetParent(taggedItem);
                        CurrentGroup = tag.TagLabel;
                        CurrentGroupVisibility = Visibility.Visible;
                        UpdateTagSuggestions();
                    }
                    TagSearchQuery = "";
                }
            }
            else if(TagSearchSuggestions != null && TagSearchSuggestions.Count > 0)
            {
                if (!(_tagTabIndex <= TagSearchSuggestions.Count() - 1))
                {
                    _tagTabIndex = 0;
                }
                TagSearchQuery = TagSearchSuggestions[_tagTabIndex].TagLabel + ' ';
                _tagTabIndex++;
            }
        }

        /// <summary>
        /// Applies the tag or enters the tag, if it is a parent
        /// </summary>
        private void ApplyTagOrEnterParent()
        {
            ITagable tag = TagSearchSuggestions.SingleOrDefault<ITagable>(t => t.TagLabel == TagSearchQuery.Trim(' '));
            if (tag != null)
            {
                if (tag.ParentId == 0 && (tag.TagId == 1 || tag.ChildrenCount > 0))
                {
                    _tagHelper.SetParent((Tag)tag);
                    CurrentGroup = tag.TagLabel;
                    CurrentGroupVisibility = Visibility.Visible;
                }
                else
                {
                    _tagHelper.AddTag(tag);
                    _assetController.AttachTag(tag);
                    AppliedTags = _tagHelper.GetAppliedTags(false);
                }
                TagSearchQuery = "";
                _tagTabIndex = 0;
                UpdateAll();
            }
            else
            {
                //TODO Notify the user, that the input is not a tag
                Console.WriteLine("Not a tag");
            }
        }

        public override void UpdateOnFocus()
        {
            OnPropertyChanged(nameof(TagSearchSuggestions));
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Identifier));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(TagSearchQuery));
            UpdateAll();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Runs the tagsearch process.
        /// </summary>
        private void TagSearch()
        {
            UpdateTagSuggestions();
        }

        /// <summary>
        /// Clears the input in the searchbar
        /// </summary>
        private void ClearInput()
        {
            if (_tagHelper.IsParentSet())
            {
                _tagHelper.SetParent(null);
                CurrentGroup = "";
                CurrentGroupVisibility = Visibility.Collapsed;
                TagSearchQuery = "";
                UpdateTagSuggestions();
            }
        }

        /// <summary>
        /// Runs the tag process
        /// </summary>
        private void UpdateTagSuggestions()
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
            UpdateTagRelationsOfFields(AppliedTags);
            OnPropertyChanged(nameof(AppliedTags));
            OnPropertyChanged(nameof(NonHiddenFieldList));
            OnPropertyChanged(nameof(HiddenFieldList));
        }

        /// <summary>
        /// Updates the fields to show their connections to the given tags
        /// </summary>
        /// <param name="tagsList"></param>
        private void UpdateTagRelationsOfFields(ObservableCollection<ITagable> tagsList)
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
            else if (Features.Main.CurrentDepartment.ID == 0)
            {
                Features.AddNotification(new Notification("Please select another department than \"All departments\"", Notification.WARNING));
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
                    if (!string.IsNullOrEmpty(field.Content))
                    {
                        bool check = field.Content.All(char.IsDigit);
                        if (!check)
                        {
                            Features.AddNotification(
                                new Notification("The field " + field.Label + " cannot contain letters",
                                    Notification.WARNING));
                            return false;
                        }
                    }
                }

                if (field.Type == Field.FieldType.Date && string.Equals(field.Content,"today"))
                {
                    field.Content = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                }
            }

            return true;
        }

        private void EditFieldConfirmed(object sender, PromptEventArgs e)
        {
            if (e is FieldEditPromptEventArgs args)
            {
                args.OldField.Label = args.NewField.Label;
                args.OldField.Required = args.NewField.Required;
                args.OldField.Type = args.NewField.Type;
                args.OldField.Content = args.NewField.Content;
                UpdateAll();
            }
        }

        #endregion
    }
}