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
        private AssetController _assetController;
        private TagListController _tagListController;
        private bool _isEditing;

        public ICommand AddFieldCommand { get; set; }
        
        public ICommand RemoveFieldCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand SaveMultipleCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public List<ITagable> CurrentlyAddedTags
        {
            get => _assetController.CurrentlyAddedTags;
            set => _assetController.CurrentlyAddedTags = value;
        }

        public ObservableCollection<Field> NonHiddenFieldList { get; set; }
        
        public ObservableCollection<Field> HiddenFieldList { get; set; }

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

        public AssetEditorViewModel(Asset asset, IAssetRepository assetRepository, TagListController tagListController,MainViewModel main)
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
            NonHiddenFieldList = new ObservableCollection<Field>(_assetController.FieldList.Where(f => f.IsHidden == false));
            HiddenFieldList = new ObservableCollection<Field>(_assetController.FieldList.Where(f => f.IsHidden == true));
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
            SaveAsset();
            if (_main.ContentFrame.CanGoBack)
            {
                Console.WriteLine("GOBack");
                _main.ContentFrame.GoBack();
            }
            else
            {
                Console.WriteLine("Navigated");
                _main.ContentFrame.Navigate(new AssetList(_main, new AssetRepository(), new PrintHelper()));
            }
        }

        public void SaveAsset()
        {
            if (_isEditing)
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

        public void PromptForCustomField()
        {
            _main.DisplayPrompt(new CustomField("Add field", AddCustomField));
        }

        public void AddCustomField(object sender, PromptEventArgs e)
        {
            if(e is FieldInputPromptEventArgs)
            {
                Field returnedField = ((FieldInputPromptEventArgs)e).Field;
                NonHiddenFieldList.Insert(NonHiddenFieldList.Count, returnedField);
            }
        }
        
        public void RemoveField(object sender)
        {
            Console.WriteLine(sender.GetType());
            if (sender is Field field)
            {
                Console.WriteLine(NonHiddenFieldList.Count);
                Console.WriteLine(field.Label);
                if (!field.IsCustom)
                {
                    Console.WriteLine("Adding... current:" + HiddenFieldList.Count);
                    HiddenFieldList.Add(field);
                    Console.WriteLine("Added to hidden field now:" + HiddenFieldList.Count);
                }
                NonHiddenFieldList.Remove(field);
                Console.WriteLine(NonHiddenFieldList.Count);   
            }
            else
            {
                Console.WriteLine("Not a field");
            }
        }

        public void Cancel()
        {
            if (_main.ContentFrame.CanGoBack)
            {
                Console.WriteLine("GOBack");
                _main.ContentFrame.GoBack();
            }
            else
            {
                Console.WriteLine("Navigated");
                _main.ContentFrame.Navigate(new AssetList(_main, new AssetRepository(), new PrintHelper()));
            }
        }
    }
}