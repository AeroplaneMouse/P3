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
        private ulong _parentID;
        private ulong _departmentID;
        private int _numOfChildren;

        public string Name 
        {
            get => this._name;
            set 
            {
                if (this.Name != null)
                    this.Changes["Name"] = this.Name;
                this._name = value.ToLower();
            }
        }

        public string Color 
        {
            get => this._color;
            set 
            {
                if (this.Color != null)
                {
                    this.Changes["Color"] = this.Color;
                }

                this._color = value;
            }
        }

        public ulong ParentID 
        {
            get => this._parentID;
            set 
            {
                if (this.ParentID > 0)
                {
                    this.Changes["ParentID"] = this.ParentID;
                }

                this._parentID = value;
            }
        }

        public ulong DepartmentID 
        {
            get => this._departmentID;
            set 
            {
                if (this.DepartmentID > 0)
                {
                    this.Changes["DepartmentID"] = this.DepartmentID;
                }

                this._departmentID = value;
            }
        }

        public int NumOfChildren 
        {
            get => this._numOfChildren;
            set 
            {
                if (this.NumOfChildren >= 0)
                {
                    this.Changes["Color"] = this.NumOfChildren;
                }

                this._numOfChildren = value;
            }
        }

        public Tag()
        {
            
        }

        /*Constructor used by DB*/
        private Tag(ulong id, string name, ulong department_id, ulong parent_id, string color, int numOfChildren, DateTime created_at, DateTime updated_at, string serializedField)
        {
            ID = id;
            Name = name;
            DepartmentID = department_id;
            ParentID = parent_id;
            Color = color;
            NumOfChildren = numOfChildren;
            this.SerializedFields = serializedField;
            CreatedAt = created_at;
            UpdatedAt = updated_at;
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
        public ulong ParentId => ParentID;
        public List<ITagable> Children { get; set; } = new List<ITagable>();
        public string TagColor { 
            get => Color;
            set => Color = value; }
        public SolidColorBrush TagFontColor => Notification.GetForegroundColor(TagColor);

        #endregion
    }
}
