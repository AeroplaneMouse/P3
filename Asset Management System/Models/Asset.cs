﻿using System;
using System.Collections.Generic;

namespace Asset_Management_System.Models
{
    [Serializable]
    public class Asset : DoesContainFields
    {
        public Asset(){ }

        private Asset(ulong id, string name, string description, ulong department_id)
        {
            ID = id;
            Name = name;
            Description = description;
            DepartmentID = department_id;
            CreatedAt = DateTime.Now;
            FieldsList = new List<Field>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public ulong DepartmentID { get; set; }

        public ulong TagID { get; set; }
    }
}