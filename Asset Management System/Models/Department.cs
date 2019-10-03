﻿
namespace Asset_Management_System.Models
{
    public class Department
    {
        public Department(string name)
        {
            this.Name = name;
        }

        private Department(long id, string name){
            this.ID = id;
            this.Name = name;
        }

        public long ID { get; }
        public string Name { get; set; }

        public override string ToString() => Name;
    }
}