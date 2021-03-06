﻿using AMS.Controllers.Interfaces;
using AMS.Interfaces;
using AMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace AMS.ViewModels
{
    public class AssetDetailsViewModel : Base.BaseViewModel
    {
        private IAssetController _assetController { get; set; }

        public string Name => _assetController.ControlledAsset.Name;
        public string Identifier => _assetController.ControlledAsset.Identifier;
        public string Description => _assetController.ControlledAsset.Description;
        public ObservableCollection<ITagable> TagList => new ObservableCollection<ITagable>
        (
            // Only show parent tag without children and children
            _assetController.CurrentlyAddedTags.Where(t => t.ParentId != 0 || (t.ParentId == 0 && t.NumberOfChildren == 0 && t.TagId != 1))
        );

        public ObservableCollection<Field> FieldList => new ObservableCollection<Field>
        (
            // Only show fields with content
            _assetController.ControlledAsset.FieldList.Where(p => p.IsHidden == false && !string.IsNullOrEmpty(p.Content)).ToList()
        );

        public AssetDetailsViewModel(IAssetController assetController)
        {
            _assetController = assetController;
            UpdateOnFocus();
        }

        public override void UpdateOnFocus()
        {
            UpdateTagRelations();
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Identifier));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(TagList));
            OnPropertyChanged(nameof(FieldList));
        }

        /// <summary>
        /// For each field attached to the asset, find all the tags that they come from
        /// </summary>
        private void UpdateTagRelations()
        {
            foreach (var field in FieldList)
            {
                field.TagList = new List<ITagable>();

                foreach (var id in field.TagIDs)
                {
                    foreach(ITagable tag in TagList)
                    {
                        if (tag.ParentId == id || tag.TagId == id)
                            field.TagList.Add(tag);
                    }
                }
            }
        }
    }
}
