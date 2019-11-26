using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Text;
using AMS.Interfaces;
using Newtonsoft.Json;

namespace AMS.Models
{
    public class Tag :FieldContainer, ITagable
    {
        public Tag()
        {
            
        }

        /*Constructor used by DB*/
        private Tag(ulong id, string name, ulong department_id, ulong parent_id, string color, int numOfChildren, DateTime created_at, DateTime updated_at, string SerializedField)
        {
            ID = id;
            Name = name;
            DepartmentID = department_id;
            ParentID = parent_id;
            TagColor = color;
            NumOfChildren = numOfChildren;
            this.SerializedFields = SerializedField;
            CreatedAt = created_at;
            UpdatedAt = updated_at;
            DeSerializeFields();
        }
        
        public string Name { get; set; }

        public ulong ParentID { get; set; }

        public ulong DepartmentID { get; set; }

        public int NumOfChildren { get; set; }

        public override string ToString() => Name;
        
        /// <summary>
        /// Loads the fields from the serialized fields property.
        /// </summary>
        /// <returns>Load successfull</returns>
        public bool DeSerializeFields()
        {
            if (!string.IsNullOrEmpty(this.SerializedFields))
            {
                this.FieldList =
                    JsonConvert.DeserializeObject<List<Field>>(this.SerializedFields);
                return true;
            }

            return false;
        }
        #region From ITagable

        public ulong TagId => ID;
        public Type TagType => this.GetType();
        public string TagLabel => Name;
        public ulong ParentId => ParentID;
        public int ChildrenCount => NumOfChildren;
        public List<ITagable> Children { get; set; }
        public string TagColor { get; set; }
        public SolidColorBrush TagFontColor => Notification.GetForegroundColor(TagColor);

        #endregion
    }
}