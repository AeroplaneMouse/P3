using System;
using System.Collections.Generic;

namespace Asset_Management_System.Models
{
    class Tag : DoContainFields
    {
        public Tag(string name,int departmentId,int parentId = 0)
        {
            DepartmentId = departmentId;
            CreatedAt = DateTime.Now;
            Name = name;
            FieldsList = new List<Field>();
            ParentId = parentId;

        }

        public int ID { get; }

        public int ParentId;

        public string Name { get; set; }

        public int DepartmentId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool RenameTag(string newName)
        {
            if (newName!= null)
            {
                Name = newName;    
                UpdatedAt = DateTime.Now;
            }
            else
            {
                throw new NullReferenceException();
            }

            return true;
        }
        
    }
}