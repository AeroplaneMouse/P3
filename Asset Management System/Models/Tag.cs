using System;
using System.Collections.Generic;

namespace Asset_Management_System.Models
{
    public class Tag : DoesContainFields
    {
        public Tag(string label, ulong departmentID, ulong parentID) : this(0, label, departmentID, parentID){}

        /*Constructor used by DB*/
        private Tag(ulong id, string label, ulong department_id, ulong parent_id)
        {
            ID = id;
            Label = label;
            DepartmentID = department_id;
            FieldsList = new List<Field>();
            ParentID = parent_id;
        }

        public string Label { get; set; }

        public string Color { get; set; }

        public ulong ParentID { get; set; }

        public ulong DepartmentID { get; set; }

        public override string ToString() => Label;
    }
}