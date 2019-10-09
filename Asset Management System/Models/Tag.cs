using System;
using System.Collections.Generic;

namespace Asset_Management_System.Models
{
    class Tag : DoesContainFields
    {
        public Tag(string label, ulong departmentID = 0, ulong parentID = 0)
        {
            DepartmentID = departmentID;
            CreatedAt = DateTime.Now;
            Label = label;
            FieldsList = new List<Field>();
            ParentID = parentID;
        }

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