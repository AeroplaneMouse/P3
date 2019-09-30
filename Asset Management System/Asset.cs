using System;
using System.Collections.Generic;
using System.Text;

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
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
        public int Department_ID { get; set; }
    }
}
