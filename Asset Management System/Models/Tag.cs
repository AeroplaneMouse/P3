using System;
using System.Collections.Generic;

namespace Asset_Management_System.Models
{
    class Tag : DoContainFields
    {
        public Tag(string name, int departmentId = 0, int parentId = 0)
        {
            DepartmentId = departmentId;
            CreatedAt = DateTime.Now;
            Name = name;
            FieldsList = new List<Field>();
            ParentId = parentId;
        }

        internal Tag(long id, string name, long departmentId, long parentId)
        {
            Id = id;
            DepartmentId = departmentId;
            Name = name;
            FieldsList = new List<Field>();
            ParentId = parentId;
        }

        public long ParentId;

        public string Name { get; set; }

        public long DepartmentId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool RenameTag(string newName)
        {
            if (newName != null)
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