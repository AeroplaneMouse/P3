using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.Models;

namespace AMS.Controllers
{
    public class AssetController : FieldController, IAssetController
    {
        public Asset Asset { get; set; }
        public List<ITagable> CurrentlyAddedTags { get; set; } = new List<ITagable>();
        public List<Field> FieldList { get; set; } = new List<Field>();

        private IAssetRepository _assetRepository;
        

        public AssetController(Asset asset, IAssetRepository assetRepository) : base(asset)
        {
            Asset = asset;
            DeSerializeFields();
            FieldList = asset.Fields.ToList<Field>();
            _assetRepository = assetRepository;
        }

        public bool AttachTag(ITagable tag)
        {
            CurrentlyAddedTags.Add(tag);
            if (tag.GetType() == typeof(Tag))
            {
                Tag tagWithFields = (Tag) tag;
                foreach (var tagField in tagWithFields.Fields)
                {
                    if (Asset.Fields.SingleOrDefault(assetField => assetField.Hash == tagField.Hash) == null)
                    {
                        Asset.Fields.Add(tagField);
                    }
                    else
                    {
                        Field fieldToUpdate =
                            Asset.Fields.Single(assetField => assetField.Hash == tagField.Hash);
                        fieldToUpdate.FieldPresentIn.Add(tagWithFields.ID);
                    }
                }
            }
            return CurrentlyAddedTags.Contains(tag);
        }

        public bool DetachTag(ITagable tag)
        {
            if (CurrentlyAddedTags.Contains(tag))
            {
                CurrentlyAddedTags.Remove(tag);
                if (tag is Tag currentTag)
                {
                    foreach (var field in currentTag.Fields)
                    {
                        RemoveFieldOrFieldRelations(field, currentTag);
                    }
                }
            }

            return !CurrentlyAddedTags.Contains(tag);
        }

        public bool Save()
        {
            Asset.Fields = new ObservableCollection<Field>(FieldList);
            SerializeFields();
            ulong id = 0;
            _assetRepository.AttachTags(Asset, CurrentlyAddedTags);
            _assetRepository.Insert(Asset, out id);
            return id != 0;
        }

        public bool Update()
        {
            Asset.Fields = new ObservableCollection<Field>(FieldList);
            SerializeFields();
            _assetRepository.AttachTags(Asset, CurrentlyAddedTags);
            return _assetRepository.Update(Asset);
        }

        public bool Remove()
        {
            return _assetRepository.Delete(Asset);
        }
    }
}