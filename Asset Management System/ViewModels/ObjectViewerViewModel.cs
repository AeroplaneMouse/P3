using System.Collections.ObjectModel;
using Asset_Management_System.Models;

namespace Asset_Management_System.ViewModels
{
    public class ObjectViewerViewModel
    {
        private Tag TagInput;
        private Asset AssetInput;

        public string Color;

        public ulong ParentTag;
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsTag { get; set; }

        public ObservableCollection<Field> FieldsList { get; set; }

        public ObjectViewerViewModel(MainViewModel main, DoesContainFields inputObject)
        {
            FieldsList = new ObservableCollection<Field>();

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

                Name = asset.Name;
                Description = asset.Description;
                IsTag = false;
            }
        }
    }
}