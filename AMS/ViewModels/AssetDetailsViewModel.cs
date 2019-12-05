using AMS.Controllers.Interfaces;
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
        public ObservableCollection<ITagable> TagList => new ObservableCollection<ITagable>(
            // Only show parent tag without children and children
            _assetController.CurrentlyAddedTags.Where(t => t.ParentId != 0 || (t.ParentId == 0 && t.NumOfChildren == 0))
        );

        public ObservableCollection<Field> FieldList =>
            new ObservableCollection<Field>(_assetController.ControlledAsset.FieldList.Where(p => p.IsHidden == false && !string.IsNullOrEmpty(p.Content)).ToList());
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

        private void UpdateTagRelations()
        {
            foreach (var field in FieldList)
            {
                field.TagList = new List<ITagable>();
                foreach (var id in field.TagIDs)
                {
                    foreach(ITagable tag in TagList)
                        if (tag.ParentId == id || tag.TagId == id)
                            field.TagList.Add(tag);

                    // #CrappyCode
                    //if (TagList.FirstOrDefault(p => p.TagId == id) is Tag tag)
                    //    field.TagList.Add(tag);

                    //if (TagList.forea(p => p.ParentId == id) is Tag parentTag)
                    //    field.TagList.Add(parentTag);
                }
            }
        }
    }
}
