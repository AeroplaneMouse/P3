using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Asset_Management_System.Models
{
    class Asset : DoContainFields
    {
        public Asset(string label, string description)
        {
            Name = label;
            Description = description;
            CreatedAt = DateTime.Now;
            FieldsList = new List<Field>();
        }

        public int Id { get; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public int DepartmentID { get; set; }

        public string Fields { get; set; }

        private List<Field> _fieldsList;
        
        public void SerializeFields()
        {
            
        }

        public int TagId { get; set; }

    }
}