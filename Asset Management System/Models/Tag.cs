using System;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;

namespace Asset_Management_System.Models
{
    public class Tag : DoesContainFields
    {
        public Tag()
        {
        }

        /*Constructor used by DB*/
        private Tag(ulong id, string label, ulong department_id, ulong parent_id, string color, DateTime created_at)
        {
            ID = id;
            Label = label;
            DepartmentID = department_id;
            FieldsList = new List<Field>();
            CreatedAt = created_at;
            ParentID = parent_id;
            Color = color;
        }

        public string Label { get; set; }

        public string Color { get; set; }

        public ulong ParentID { get; set; }

        public ulong DepartmentID { get; set; }

        public override string ToString() => Label;
    }
}