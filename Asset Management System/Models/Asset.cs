using System;
using System.Collections.Generic;
using System.Windows.Documents;
using LinqToDB.Mapping;

namespace Asset_Management_System.Models
{
    [Table(Name = "assets")]
    public class Asset
    {

        public Asset(string name, string description)
        {
            Name = name;
            Description = description;
        }

        [PrimaryKey, Identity, Column(Name = "id")]
        public int Id { get; }

        [Column(Name = "name"), NotNull]
        public string Name { get; set; }

        [Column(Name = "description"), Nullable]
        public string Description { get; set; }

        [Column(Name = "created_at"), Nullable]
        public DateTime CreatedAt { get; set; }

        [Column(Name = "department_id"), NotNull]
        public int DepartmentID { get; set; }

        [Column(Name = "options"), NotNull]
        public string Fields { get; set; }

        private List<Field> _fieldsList;
        public void SerializeFields()
        {
            
        }
        
        

    }
}