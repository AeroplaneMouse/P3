﻿using System;
using System.Collections.Generic;

namespace Asset_Management_System.Models
{
    [Serializable]
    public class Asset : DoesContainFields
    {
        public Asset()
        {
            FieldsList = new List<Field>();
            this.SavePrevValues();
        }

        private Asset(ulong id, string name, string description, ulong department_id, DateTime created_at)
        {
            ID = id;
            Name = name;
            Description = description;
            DepartmentID = department_id;
            CreatedAt = created_at;
            FieldsList = new List<Field>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public ulong DepartmentID { get; set; }
    }
}