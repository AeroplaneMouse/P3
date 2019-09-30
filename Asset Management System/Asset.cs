﻿using System;

namespace Asset_Management_System
{
    class Asset
    {

        public Asset()
        {
        }

        public int ID { get; }

        public string Name { get; set; }

        public Asset(string label, string description)
        {
            Label = label;
            Description = description;
        }

        public int ID { get; }
        
        public string Label { get; set; }
        
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public int DepartmentID { get; set; }

        public string Fields { get; set; }
    }
}