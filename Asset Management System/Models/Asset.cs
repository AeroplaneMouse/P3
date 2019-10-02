using System;
using System.Collections.Generic;

namespace Asset_Management_System.Models
{
    [Serializable]
    class Asset : DoContainFields
    {
        public Asset(long id, string label, string description)
        {
            ID = id;
            Label = label;
            Description = description;
            CreatedAt = DateTime.Now;
            FieldsList = new List<Field>();
        }

        public long ID { get; }
        
        public string Label { get; set; }
        
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public int DepartmentID { get; set; }

        public int TagId { get; set; }

    }
}