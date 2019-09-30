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
        
        public int Department_ID { get; set; }
        
        public DateTime Created_at { get; set; }
        
        public DateTime Updated_at { get; set; }

        public Field[] Fields { get; set; }

    }
}
