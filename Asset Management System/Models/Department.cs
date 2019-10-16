
using System;

namespace Asset_Management_System.Models
{
    public class Department : Model
    {
        public Department()
        {
            
        }
        
        private Department(ulong id, string name, DateTime created_at, DateTime updated_at){
            ID = id;
            Name = name;
            CreatedAt = created_at;
            UpdatedAt = updated_at;
            SavePrevValues();
        }

        public string Name { get; set; }

        public override string ToString() => Name;
    }
}