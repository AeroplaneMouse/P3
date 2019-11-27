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
        private Asset _asset { get; set; }
        public string Name => _asset.Name;
        public string Identifier => _asset.Identifier;
        public string Description => _asset.Description;
        public List<ITagable> TagList { get; set; }

        public ObservableCollection<Field> FieldList =>
            new ObservableCollection<Field>(_asset.FieldList.Where(p => p.IsHidden == false && !string.IsNullOrEmpty(p.Content)).ToList());
        public AssetDetailsViewModel(Asset asset, List<ITagable> tagList)
        {
            TagList = tagList;
            _asset = asset;
            UpdateTagRelations();
        }
        
        private void UpdateTagRelations()
        {
            foreach (var field in FieldList)
            {
                field.TagList = new List<Tag>();
                foreach (var id in field.TagIDs)
                {
                    if (TagList.SingleOrDefault(p => p.TagId == id) is Tag tag)
                    {
                        field.TagList.Add(tag);
                    }
                }
            }
        }
    }
}
