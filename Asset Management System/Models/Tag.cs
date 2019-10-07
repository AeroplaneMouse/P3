using System;
using System.Collections.Generic;

namespace Asset_Management_System.Models
{
    class Tag : DoesContainFields
    {
        public Tag(string name, int departmentID = 0, int parentID = 0)
        {
            DepartmentID = departmentID;
            CreatedAt = DateTime.Now;
            Name = name;
            FieldsList = new List<Field>();
            ParentID = parentID;
        }

        private Tag(long id, string name, long departmentId, long parentId)
        {
            ID = id;
            Name = name;
            DepartmentID = departmentID;
            FieldsList = new List<Field>();
            ParentID = parentID;
        }

        public long ParentID;

        public long DepartmentID { get; set; }

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

        public override string ToString() => Name;
    }
}