﻿using System;
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
        private Tag(ulong id, string name, ulong department_id, ulong parent_id, string color, DateTime created_at, DateTime updated_at)
        {
            ID = id;
            Name = name;
            DepartmentID = department_id;
            ParentID = parent_id;
            Color = color;
            FieldsList = new List<Field>();
            CreatedAt = created_at;
            UpdatedAt = updated_at;
            SavePrevValues();
        }

        public string Name { get; set; }

        public string Color { get; set; }

        public ulong ParentID { get; set; }

        public ulong DepartmentID { get; set; }

        public override string ToString() => Name;
    }
}