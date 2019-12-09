using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
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
        private IAssetController _assetController { get; set; }
        private bool _isEditing { get; set; }
        private string _tagSearchQuery { get; set; }
        private TagHelper _tagHelper { get; set; }
        private int _tagTabIndex { get; set; }

        public ObservableCollection<Field> NonHiddenFieldList => new ObservableCollection<Field>(_assetController.NonHiddenFieldList);
        public ObservableCollection<Field> HiddenFieldList => new ObservableCollection<Field>(_assetController.HiddenFieldList);
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
                if (!_tagSearchQuery.EndsWith(' '))
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
        public ICommand BackspaceCommand { get; set; }

        public AssetEditorViewModel(IAssetController assetController, TagHelper tagHelper)
        {
            _assetController = assetController;

            _tagHelper = tagHelper;
            _tagHelper.CanApplyParentTags = false;
            _tagHelper.SetAppliedTags(new ObservableCollection<ITagable>(_assetController.CurrentlyAddedTags));
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
            });

            RemoveCommand = new RelayCommand(() =>
            {
                Features.DisplayPrompt(new Views.Prompts.Confirm(
                    $"Are you sure you want to remove {_assetController.Name}?", (sender, e) =>
                    {
                        if (e.Result)
                        {
                            _assetController.Remove();
                            Features.Navigate.To(Features.Create.AssetList());
                        }
                    }));
            });

            InsertNextOrSelectedSuggestionCommand =
                new RelayCommand<object>((parameter) => InsertNextOrSelectedSuggestion(parameter));
            ClearInputCommand = new RelayCommand(ClearInput);
            BackspaceCommand = new RelayCommand<object>((parameter) => RemoveCharacterOrExitTagMode(parameter as TextBox));

            UpdateAll();
        }

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
            }
            UpdateAll();
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
            }
            UpdateAll();
        }

        /// <summary>
        /// Runs the cancel command, and returns.
        /// </summary>
        public void Cancel()
        {
            _assetController.RevertChanges();
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
                    tag = (ITagable) input;
                }

                if (tag != null)
                {
                    if (_tagHelper.IsParentSet() || (tag.NumberOfChildren == 0 && tag.TagId != 1))
                    {
                        _tagHelper.AddTag(tag);
                        _assetController.AttachTag(tag);
                        _assetController.AttachTag(_tagHelper.GetParent());
                        AppliedTags = _tagHelper.GetAppliedTags(false);
                        UpdateTagSuggestions();
                    }
                    else
                    {
                        // So we need to switch to a group of tags.
                        Tag taggedItem = (Tag) tag;
                        _tagHelper.SetParent(taggedItem);
                        CurrentGroup = tag.TagLabel;
                        CurrentGroupVisibility = Visibility.Visible;
                        UpdateTagSuggestions();
                    }

                    TagSearchQuery = "";
                }
            }
            else if (TagSearchSuggestions != null && TagSearchSuggestions.Count > 0)
            {
                if (!(_tagTabIndex <= TagSearchSuggestions.Count() - 1))
                {
                    _tagTabIndex = 0;
                }

                TagSearchQuery = TagSearchSuggestions[_tagTabIndex].TagLabel + ' ';
                _tagTabIndex++;
            }
            UpdateAll();
        }

        /// <summary>
        /// Applies the tag or enters the tag, if it is a parent
        /// </summary>
        private void ApplyTagOrEnterParent()
        {
            ITagable tag = TagSearchSuggestions.SingleOrDefault<ITagable>(t => t.TagLabel == TagSearchQuery.Trim(' '));
            if (tag != null)
            {
                if (tag.ParentId == 0 && (tag.TagId == 1 || tag.NumberOfChildren > 0))
                {
                    // Set parent tag with children or user parent
                    _tagHelper.SetParent((Tag) tag);
                    CurrentGroup = tag.TagLabel;
                    CurrentGroupVisibility = Visibility.Visible;
                }
                else
                {
                    // Attach tag and parent
                    _tagHelper.AddTag(tag);
                    _assetController.AttachTag(tag);
                    _assetController.AttachTag(_tagHelper.GetParent());
                    AppliedTags = _tagHelper.GetAppliedTags(false);
                }

                TagSearchQuery = "";
                _tagTabIndex = 0;
                UpdateAll();
            }
            else
            {
                string message;
                if (TagSearchQuery == String.Empty)
                    message = $"It is not possible to attach a parent tag that have children to an asset.";
                else
                    message =
                        $"{TagSearchQuery} is not a tag. To use it, you must first create a tag called {TagSearchQuery}.";

                Features.AddNotification(new Notification(message, background: Notification.WARNING),
                    displayTime: 3500);
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
            UpdateTagRelationsOnFields(AppliedTags);
            OnPropertyChanged(nameof(AppliedTags));
            OnPropertyChanged(nameof(NonHiddenFieldList));
            OnPropertyChanged(nameof(HiddenFieldList));
            _assetController.LoadFields();
        }

        /// <summary>
        /// Updates the fields to show their connections to the given tags
        /// </summary>
        /// <param name="tagList"></param>
        private void UpdateTagRelationsOnFields(ObservableCollection<ITagable> tagList)
        {
            // Runs through the list of tagID's, and adds the tag with the same tagID to the TagList on the field.
            foreach (var field in HiddenFieldList)
            {
                UpdateTagRelationsOnSingleField(field, tagList);
            }

            foreach (var field in NonHiddenFieldList)
            {
                UpdateTagRelationsOnSingleField(field, tagList);
            }
        }

        private void UpdateTagRelationsOnSingleField(Field field, ObservableCollection<ITagable> tagList)
        {
            field.TagList = new List<ITagable>();
            foreach (var id in field.TagIDs)
            {
                foreach (ITagable tag in tagList.Where(p => p.TagId == id || p.ParentId == id))
                {
                    field.TagList.Add(tag);
                }
            }

            if (!field.TagList.Any() && !field.IsCustom)
            {
                _assetController.RemoveField(field);
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
                Features.AddNotification(new Notification("The field " + "Name" + " is required and empty",
                    Notification.WARNING));
                return false;
            }
            else if (Features.Main.CurrentDepartment.ID == 0 && !_isEditing)
            {
                Features.AddNotification(new Notification("Please select another department than \"All departments\"",
                    Notification.WARNING));
                return false;
            }

            //Verifies whether fields contains correct information, or the required information.
            List<Field> completeList = HiddenFieldList.ToList();
            completeList.AddRange(NonHiddenFieldList.ToList());

            foreach (var field in completeList)
            {
                if (field.Required && string.IsNullOrEmpty(field.Content) && !field.IsHidden)
                {
                    Features.AddNotification(new Notification("The field " + field.Label + " is required and empty",
                        Notification.WARNING));
                    return false;
                }if (field.Required && field.Type == Field.FieldType.Date && field.Content == "Current date" && field.Content == "Empty" && !field.IsHidden)
                {
                    Features.AddNotification(new Notification("The field " + field.Label + " is required and empty",
                        Notification.WARNING));
                    return false;
                }

                if (field.Type == Field.FieldType.NumberField)
                {
                    //Checks for ^(Not) the result, as and then returns true if it finds an element that does not match the rule.
                    Regex regex = new Regex("[^0-9.+-/,]+");
                    if (!string.IsNullOrEmpty(field.Content))
                    {
                        //Checks whether the match is true
                        bool check = !regex.IsMatch(field.Content);
                        //Checks if the match returned false
                        if (!check)
                        {
                            Features.AddNotification(
                                new Notification("The field " + field.Label + " cannot contain letters",
                                    Notification.WARNING));
                            return false;
                        }
                    }
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

        private void RemoveCharacterOrExitTagMode(TextBox textBox)
        {
            if (TagSearchQuery.Length > 0)
            {
                int cursorIndex = textBox.CaretIndex;
                if (cursorIndex > 0)
                {
                    TagSearchQuery = TagSearchQuery.Remove(cursorIndex - 1, 1);
                    textBox.CaretIndex = cursorIndex - 1;
                }
            }
            else
            {
                ClearInput();
            }
        }
    }
}