using System;
using System.Collections.Generic;

namespace Asset_Management_System.Models
{
    [Serializable]
    class Asset : DoesContainFields
    {
        public Asset(){ }

        private Asset(long id, string name, string description, long department_id)
        {
            ID = id;
            Name = name;
            Description = description;
            DepartmentID = department_id;
            CreatedAt = DateTime.Now;
            FieldsList = new List<Field>();
        }

        public string Description { get; set; }

        public long DepartmentID { get; set; }

        public int TagID { get; set; }
    }
}