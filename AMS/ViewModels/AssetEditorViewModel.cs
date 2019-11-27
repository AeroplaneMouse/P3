using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using AMS.Controllers;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories;
using AMS.Database.Repositories.Interfaces;
using AMS.Events;
using AMS.Helpers;
using AMS.Interfaces;
using AMS.Models;
using AMS.ViewModels.Base;
using AMS.Views;
using AMS.Views.Prompts;

namespace AMS.ViewModels
{
    public class AssetEditorViewModel : BaseViewModel
    {
        private IAssetController _assetController;
        private ITagListController _tagListController;
        private bool _isEditing;
        private TagHelper _tagHelper;

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
        
        public ObservableCollection<Field> NonHiddenFieldList => new ObservableCollection<Field>(_assetController.NonHiddenFieldList);
        public ObservableCollection<Field> HiddenFieldList => new ObservableCollection<Field>(_assetController.HiddenFieldList);

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

        private string _tagString;

        private string _tagSearchQuery;
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
        public ObservableCollection<ITagable> TagSearchSuggestions { get; set; }
        public ITagable TagParent { get; set; }
        public ObservableCollection<ITagable> AppliedTags { get; set; }

        public List<Tag> TagList { get; set; }
        
        public AssetEditorViewModel(IAssetController assetController, ITagListController tagListController, TagHelper tagHelper)
        {
            _assetController = assetController;
            _tagListController = tagListController;

            _tagHelper = tagHelper;
            _tagHelper.CanApplyParentTags = false;
            _tagHelper.SetCurrentTags(new ObservableCollection<ITagable>(_assetController.CurrentlyAddedTags));
            AppliedTags = _tagHelper.GetAppliedTags(false);
            
            _isEditing = (_assetController.Asset.ID != 0);
            if (_isEditing)
                Title = "Edit asset";
            else
            {
                Title = "Add asset";
            }

            //Commands
            SaveCommand = new RelayCommand(() => SaveAndExist());
            SaveMultipleCommand = new RelayCommand(() => SaveAsset());
            AddFieldCommand = new Base.RelayCommand(() => PromptForCustomField());
            CancelCommand = new Base.RelayCommand(() => Cancel());
            RemoveFieldCommand = new RelayCommand<object>((parameter) => RemoveField(parameter));
            //RemoveTagCommand = new RelayCommand<object>((parameter) => RemoveTag(parameter));
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

        public void SaveAndExist()
        {
            SaveAsset(false);

            Features.Navigate.Back();

        }

        public void SaveAsset(bool multiAdd = true)
        {
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

        public void PromptForCustomField()
        {
            Features.DisplayPrompt(new CustomField("Add field", AddCustomField, true));
        }

        public void AddCustomField(object sender, PromptEventArgs e)
        {
            if(e is FieldInputPromptEventArgs args)
            {
                _assetController.AddField(args.Field);
                UpdateAll();
            }
        }
        
        public void RemoveField(object sender)
        {
            if (sender is Field field)
            {
                _assetController.RemoveField(field);
                UpdateAll();
                
            }
        }

        public void Cancel()
        {
            if (Features.Navigate.Back() == false)
            {
                Features.Navigate.To(Features.Create.AssetList());
            }
        }

        private void TagSearch()
        {
            TagSearchProcess();
        }

        /// <summary>
        /// Attach given tag to the asset
        /// </summary>
        /// <param name="tag"></param>
        public void AutoTag()
        {
            Console.WriteLine("Tab clicked!");
            
            if (TagSearchSuggestions != null && TagSearchSuggestions.Count > 0)
            {
                ITagable tag = TagSearchSuggestions[0];
                
                Console.WriteLine("Found: "+tag.TagLabel);
            
                if (_tagHelper.IsParentSet() || (tag.ChildrenCount == 0 && tag.TagId != 1)){
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
        
        private void ClearInput()
        {
            if (_tagHelper.IsParentSet()){
                _tagHelper.Parent(null);
                CurrentGroup = "";
                CurrentGroupVisibility = Visibility.Collapsed;
                TagSearchQuery = "";
                TagSearchProcess();
            }
        }

        private void TagSearchProcess()
        {
            TagSearchSuggestions = new ObservableCollection<ITagable>(_tagHelper.Suggest(_tagSearchQuery));

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
        /// Detach tag with given tagID from asset
        /// </summary>
        /// <param name="tagID"></param>
        public void RemoveTag(object tagID)
        {
            // Display notification if given ID is not ulong
            if (!ulong.TryParse(tagID.ToString(), out var id))
            {
                Features.AddNotification(new Notification("Invalid Tag ID", Notification.ERROR));
                return;
            }
            
            ITagable tag = _assetController.CurrentlyAddedTags.Find(T => T.TagId == id);
            
            // Display notification if tag was not removed
            if(!_assetController.DetachTag(tag))
                Features.AddNotification(new Notification("Could not remove tag", Notification.ERROR));
            UpdateAll();
        }

        private void UpdateAll()
        {
            OnPropertyChanged(nameof(AppliedTags));
            OnPropertyChanged(nameof(NonHiddenFieldList));
            OnPropertyChanged(nameof(HiddenFieldList));
        }
    }
}