using System;
using System.Collections.Generic;

namespace Asset_Management_System.Models
{
    [Serializable]
    class Asset : DoesContainFields
    {
        public Asset(){ }

        public Asset(long id, string name, string description, long departmentID)
        {
            ID = id;
            Name = name;
            Description = description;
            DepartmentID = departmentID;
            CreatedAt = DateTime.Now;
            FieldsList = new List<Field>();
        }

        public string Description { get; set; }

        public long DepartmentID { get; set; }

        public int TagID { get; set; }
    }
}