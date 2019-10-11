using System;
using System.Collections.Generic;

namespace Asset_Management_System.Models
{
    public class Tag : DoesContainFields
    {
        public Tag()
        {
        }

        /*Constructor used by DB*/
        private Tag(ulong id, string label, ulong department_id, ulong parent_id, DateTime created_at)
        {
            ID = id;
            Label = label;
            DepartmentID = department_id;
            FieldsList = new List<Field>();
            ParentID = parent_id;
            CreatedAt = created_at;
            
            // Saves the value of the properties to detect changes
            if (id != 0)
            {
                SavePrevValues(); 
            }
        }

        public string Label { get; set; }

        public string Color { get; set; }

        public ulong ParentID { get; set; }

        public ulong DepartmentID { get; set; }

        public override string ToString() => Label;
    }
}