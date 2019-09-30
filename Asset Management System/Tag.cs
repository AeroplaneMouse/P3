using System;
using System.Collections.Generic;
using System.Text;

namespace Asset_Management_System
{
    class Tag
    {

        public Tag()
        {

        }

        public int ID { get; }
        
        public string Name { get; set; }
        
        public int DepartmentID { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }

        public Field[] Fields { get; set; }

    }
}
