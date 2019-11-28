using System;
using System.Collections.Generic;
using System.Linq;

namespace AMS.Models
{
    public abstract class Model
    {
        public ulong ID { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
        public string CreatedAtString => CreatedAt.ToString("u").TrimEnd('Z');
        public string UpdatedAtString => UpdatedAt.ToString("u").TrimEnd('Z');
        public Dictionary<string, object> Changes { get; set; } = new Dictionary<string, object>();
        public bool IsDirty()
        {
            return Changes.Any();
        }
    }
}
