using System;
using System.Collections.Generic;

namespace Asset_Management_System.Models
{
    [Serializable]
    class Asset : DoesContainFields
    {
        public Asset(){ }

        public Asset(long id, string label, string description, long department_id)
        {
            Id = id;
            Label = label;
            Description = description;
            DepartmentID = department_id;
            CreatedAt = DateTime.Now;
            FieldsList = new List<Field>();
        }

        public string Label { get; set; }
        
        public string Description { get; set; }

        public long DepartmentID { get; set; }

        public int TagId { get; set; }

    }
}