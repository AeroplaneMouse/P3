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
        private MainViewModel _main;
        private IAssetController _assetController;
        private ITagListController _tagListController;
        private bool _isEditing;

        public ICommand AddFieldCommand { get; set; }
        public ICommand RemoveFieldCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand SaveMultipleCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public List<ITagable> CurrentlyAddedTags => _assetController.CurrentlyAddedTags;

        public ObservableCollection<Field> NonHiddenFieldList => new ObservableCollection<Field>(_assetController.NonHiddenFieldList);
        public ObservableCollection<Field> HiddenFieldList => new ObservableCollection<Field>(_assetController.HiddenFieldList);

        public string Name
        {
            get => _assetController.Asset.Name;
            set => _assetController.Asset.Name = value;
        }

        public string Identifier
        {
            get => _assetController.Asset.Identifier;
            set => _assetController.Asset.Identifier = value;
        }

        public string Description
        {
            get => _assetController.Asset.Identifier;
            set => _assetController.Asset.Description = value;
        }

        public string Title { get; set; }

        public AssetEditorViewModel(Asset asset, IAssetRepository assetRepository, ITagListController tagListController, MainViewModel main)
        {
            _main = main;
            _isEditing = (asset != null);
            if (_isEditing)
                Title = "Edit asset";
            else
            {
                Title = "Add asset";
                asset = new Asset();
            }

            _assetController = new AssetController(asset, assetRepository);
            _tagListController = tagListController;


            //Commands
            SaveCommand = new RelayCommand(() => SaveAndExist());
            SaveMultipleCommand = new RelayCommand(() => SaveAsset());
            AddFieldCommand = new Base.RelayCommand(() => PromptForCustomField());
            CancelCommand = new Base.RelayCommand(() => Cancel());
            RemoveFieldCommand = new RelayCommand<object>((parameter) => RemoveField(parameter));
        }

        public void SaveAndExist()
        {
            SaveAsset(false);
            if (_main.ContentFrame.CanGoBack)
            {
                _main.ContentFrame.GoBack();
            }
            else
            {
                _main.ContentFrame.Navigate(new AssetList(_main, new AssetRepository(), new PrintHelper(), new CommentListController(_main.CurrentSession, new CommentRepository())));
            }
        }

        public void SaveAsset(bool multiAdd = true)
        {
            if (_isEditing)
            {
                if (!multiAdd)
                {
                    _assetController.Update();
                    _main.AddNotification(new Notification("Asset updated", Notification.APPROVE));
                }
                else
                {
                    _assetController.Save();
                    _main.AddNotification(new Notification("Asset added", Notification.APPROVE));
                }
            }
            else
            {
                _assetController.Save();
                _main.AddNotification(new Notification("Asset added", Notification.APPROVE));
            }
        }

        public void PromptForCustomField()
        {
            _main.DisplayPrompt(new CustomField("Add field", AddCustomField, true));
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
            if (_main.ContentFrame.CanGoBack)
            {
                _main.ContentFrame.GoBack();
            }
            else
            {
                _main.ContentFrame.Navigate(new AssetList(_main, new AssetRepository(), new PrintHelper(), new CommentListController(_main.CurrentSession, new CommentRepository())));
            }
        }
    }
}