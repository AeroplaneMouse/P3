using System;

namespace AMS.Models
{
    public abstract class Model
    {
        public ulong ID { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
    }
}
