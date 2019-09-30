using System;
using System.Collections.Generic;

namespace Asset_Management_System.Models
{
    class Tag : DoContainFields
    {
        public Tag(int departmentId,string name)
        {
            DepartmentID = departmentId;
            CreatedAt = DateTime.Now;
            Name = name;
            FieldsList = new List<Field>();
        }

        public int ID { get; }

        public string Name { get; set; }

        public int DepartmentID { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
        
    }
}