using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

        private string _name;
        private string _identifier;
        private string _description;

        public ICommand AddFieldCommand { get; set; }
        public ICommand RemoveFieldCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand SaveMultipleCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand RemoveTagCommand { get; set; }
        public ICommand AddTagCommand { get; set; }

        public ObservableCollection<ITagable> CurrentlyAddedTags => new ObservableCollection<ITagable>(_assetController.CurrentlyAddedTags);

        public ObservableCollection<Field> NonHiddenFieldList => new ObservableCollection<Field>(_assetController.NonHiddenFieldList);
        public ObservableCollection<Field> HiddenFieldList => new ObservableCollection<Field>(_assetController.HiddenFieldList);

        public string Name {
            get => _name;
            set => _name = value;
        }

        public string Identifier {
            get => _identifier;
            set => _identifier = value;
        }

        public string Description {
            get => _description;
            set => _description = value;
        }

        public string Title { get; set; }

        private string _tagString;
        public string TagString
        {
            get => _tagString;
            set
            {
                _tagString = value;
                _tagListController.Search(_tagString);
                TagList = _tagListController.TagsList;
            }
        }

        public List<Tag> TagList { get; set; }
        
        public AssetEditorViewModel(IAssetController assetController, ITagListController tagListController)
        {
            _assetController = assetController;
            _tagListController = tagListController;

            _isEditing = (_assetController.Asset != null);
            if (_isEditing)
                Title = "Edit asset";
            else
            {
                Title = "Add asset";
                _assetController.Asset = new Asset();
            }

            Name = _assetController.Asset.Name;
            Identifier = _assetController.Asset.Identifier;
            Description = _assetController.Asset.Description;

            //Commands
            SaveCommand = new RelayCommand(() => SaveAndExist());
            SaveMultipleCommand = new RelayCommand(() => SaveAsset());
            AddFieldCommand = new Base.RelayCommand(() => PromptForCustomField());
            CancelCommand = new Base.RelayCommand(() => Cancel());
            RemoveFieldCommand = new RelayCommand<object>((parameter) => RemoveField(parameter));
            RemoveTagCommand = new RelayCommand<object>((parameter) => RemoveTag(parameter));
            AddTagCommand = new RelayCommand<object>((parameter) => AddTag(parameter));
        }

        public void SaveAndExist()
        {
            SaveAsset(false);

            if (Features.Navigate.Back() == false)
            {
                Features.Navigate.To(Features.Create.AssetList());
            }
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
                OnPropertyChanged(nameof(NonHiddenFieldList));
                OnPropertyChanged(nameof(HiddenFieldList));
            }
        }
        
        public void RemoveField(object sender)
        {
            if (sender is Field field)
            {
                _assetController.RemoveField(field);
                OnPropertyChanged(nameof(NonHiddenFieldList));
                OnPropertyChanged(nameof(HiddenFieldList));
            }
        }

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
        public void AddTag(object tag)
        {
            // Display notification if given tag is not ITagable
            if (!(tag is ITagable))
            {
                Features.AddNotification(new Notification("Invalid Tag", Notification.ERROR));
                return;
            }
            
            // Attach Tag or display notification on failure
            if (!_assetController.AttachTag((ITagable)tag))
                Features.AddNotification(new Notification("Could not add tag", Notification.ERROR));
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
            OnPropertyChanged(nameof(CurrentlyAddedTags));
        }
    }
}