using System;

namespace Asset_Management_System.Models
{
    public abstract class Model
    {
        public ulong ID { get; protected set; }

        public DateTime CreatedAt;
        public DateTime ModifiedAt;
    }
}
