using System;
using System.Collections.Generic;
using System.Text;

namespace Asset_Management_System
{
    class Field
    {
        public Field()
        {

        }

        public int ID { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public string Type { get; set; }

    }
}
