using System;

namespace Asset_Management_System.Models
{
    public abstract class Model
    {
        public Int64 ID { get; protected set; }

        public String Name { get; set; }

        public DateTime CreatedAt;
    }
}
