using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Text;
using AMS.Interfaces;
using Newtonsoft.Json;

namespace AMS.Models
{
    public class Tag : FieldContainer, ITagable
    {
        private string _name;
        private string _color;
        private ulong _parentId;
        private ulong _departmentId;

        public string Name 
        {
            get => _name;
            set 
            {
                string propertyName = "Name";

                if (TrackChanges && !Changes.ContainsKey(propertyName) && _name != value)
                    Changes[propertyName] = _name;

                else if (Changes.ContainsKey(propertyName) && (string)Changes[propertyName] == value.ToLower())
                    Changes.Remove(propertyName);

                _name = value.ToLower();
            }
        }

        public string Color 
        {
            get => _color;
            set 
            {
                string propertyName = "Color";

                if (TrackChanges && !Changes.ContainsKey(propertyName) && _color != value)
                    Changes[propertyName] = _color;

                else if (Changes.ContainsKey(propertyName) && (string)Changes[propertyName] == value)
                    Changes.Remove(propertyName);

                _color = value;
            }
        }

        public ulong ParentId 
        {
            get => _parentId;
            set 
            {
                string propertyName = "ParentId";

                if (TrackChanges && !Changes.ContainsKey(propertyName) && _parentId != value)
                    Changes[propertyName] = _parentId;

                else if (Changes.ContainsKey(propertyName) && (ulong)Changes[propertyName] == value)
                    Changes.Remove(propertyName);

                _parentId = value;
            }
        }

        public ulong DepartmentID 
        {
            get => _departmentId;
            set 
            {
                string propertyName = "DepartmentId";

                if (TrackChanges && !Changes.ContainsKey(propertyName) && _departmentId != value)
                    Changes[propertyName] = _departmentId;

                else if (Changes.ContainsKey(propertyName) && (ulong)Changes[propertyName] == value)
                    Changes.Remove(propertyName);

                _departmentId = value;
            }
        }

        public int NumberOfChildren { get; set; }

        public Tag()
        {
            TrackChanges = true;
        }

        /*Constructor used by DB*/
        private Tag(ulong id, string name, ulong department_id, ulong parent_id, string color, int numOfChildren, string serializedField, string fullLabel, DateTime created_at, DateTime updated_at)
        {
            ID = id;
            Name = name;
            DepartmentID = department_id;
            ParentId = parent_id;
            Color = color;
            NumberOfChildren = numOfChildren;
            SerializedFields = serializedField;
            FullTagLabel = fullLabel;
            CreatedAt = created_at;
            UpdatedAt = updated_at;
            TrackChanges = true;
        }
        
        public override string ToString() => Name;
        
        /// <summary>
        /// Loads the fields from the serialized fields property.
        /// </summary>
        /// <returns>Load successfull</returns>
        public bool DeSerializeFields()
        {
            if (!string.IsNullOrEmpty(this.SerializedFields))
            {
                this.FieldList = JsonConvert.DeserializeObject<List<Field>>(this.SerializedFields);
                return true;
            }

            return false;
        }

        #region From ITagable

        public ulong TagId => ID;
        public Type TagType => this.GetType();
        public string TagLabel => Name;
        public string FullTagLabel { get; set; }
        public List<ITagable> Children { get; set; } = new List<ITagable>();
        public string TagColor { 
            get => Color;
            set => Color = value; 
        }
        
        public SolidColorBrush TagFontColor => Notification.GetForegroundColor(TagColor);
        
        public override bool Equals(object obj)
        {
            return obj is Tag tag && ID.Equals(tag.ID);
        }

        public bool Equals(Tag other)
        {
            return other != null && ID.Equals(other.ID);
        }

        #endregion
    }
}
