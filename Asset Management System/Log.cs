using System;
using System.Collections.Generic;
using System.Text;

namespace Asset_Management_System
{
    class Log
    {
        public Log()
        {

        }

        public int ID { get; }

        public string Description { get; set; }

        public DateTime RegistrationTime { get; set; }

        public User DoneBy { get; set; }
    }
}
