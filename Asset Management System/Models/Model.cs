using System;

namespace Asset_Management_System.Models
{
    public abstract class Model
    {
        public Int64 Id { get; protected set; }

        public DateTime CreatedAt;
    }
}
