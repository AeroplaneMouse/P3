using System;
using System.Collections.Generic;

namespace Asset_Management_System.Models
{
    public class Tag : DoesContainFields
    {
        public Tag(string name, int departmentID = 0, int parentID = 0) : this(0, name, departmentID, parentID)
        {
            CreatedAt = DateTime.Now;
        }

        /*Constructor used by DB*/
        private Tag(long id, string name, long department_id, long parent_id)
        {
            ID = id;
            Name = name;
            DepartmentID = department_id;
            FieldsList = new List<Field>();
            ParentID = parent_id;
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
                throw new NullReferenceException("Tag cannot be renamed to null");
            }

            return true;
        }

        public override string ToString() => Name;
    }
}