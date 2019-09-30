using System;

namespace Asset_Management_System
{
    class Asset
    {
        public Asset()
        {
        }

        public int ID { get; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int DepartmentID { get; set; }

        public Field[] Fields { get; set; }
    }
}