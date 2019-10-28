using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;

namespace Asset_Management_System.ViewModels
{
    public class ObjectViewerViewModel
    {
        private Tag TagInput;
        private Asset AssetInput;

        public string Color { get; set; }

        public ulong ParentTag{ get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsTag { get; set; }

        public ObservableCollection<Field> FieldsList { get; set; }
        
        public List<Tag> TagsList { get; set; }

        public ObjectViewerViewModel(MainViewModel main, DoesContainFields inputObject)
        {
            FieldsList = new ObservableCollection<Field>();
            TagsList = new List<Tag>();

            if (inputObject is Tag tag)
            {
                TagInput = tag;
                tag.DeserializeFields();
                foreach (var field in tag.FieldsList)
                    FieldsList.Add(field);

                Name = tag.Name;
                Color = tag.Color;
                ParentTag = tag.ParentID;
                IsTag = true;
            }
            else if (inputObject is Asset asset)
            {
                AssetInput = asset;
                asset.DeserializeFields();
                foreach (var field in asset.FieldsList)
                    FieldsList.Add(field);

                LoadTags();
                if (TagsList.Count > 0)
                {
                    int fieldsCount = 0;
                    foreach (var Tag in TagsList)
                    {
                        foreach (var field in Tag.FieldsList)
                        {
                            fieldsCount++;
                            FieldsList.Add(field);
                        }
                    }
                    Console.WriteLine("Tags add " + fieldsCount + " fields");
                }
                Name = asset.Name;
                Description = asset.Description;
                IsTag = false;
            }
        }

        /// <summary>
        /// Loads the tags attached to this asset
        /// </summary>
        private void LoadTags()
        {    
            AssetRepository rep = new AssetRepository();
            TagsList = rep.GetAssetTags(AssetInput);
            foreach (var tag in TagsList)
            {
                tag.DeserializeFields();
            }
            Console.WriteLine("Found " + TagsList.Count + " tags");
        }
    }
}