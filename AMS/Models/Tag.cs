﻿using System;
using System.Collections.Generic;
using System.Text;
using AMS.Interfaces;

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
            Color = color;
            NumOfChildren = numOfChildren;
            this.SerializedFields = SerializedField;
            CreatedAt = created_at;
            UpdatedAt = updated_at;
        }
        
        public string Name { get; set; }

        public string Color { get; set; }

        public ulong ParentID { get; set; }

        public ulong DepartmentID { get; set; }

        public int NumOfChildren { get; set; }

        public override string ToString() => Name;
        #region From ITagable
        public ulong TagId { get; }
        public Type TagType => this.GetType();
        public string TagLabel => Name;
        public List<ITagable> Children { get; set; }
        #endregion
    }
}
