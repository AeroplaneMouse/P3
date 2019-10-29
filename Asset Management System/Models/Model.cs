using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Windows.Documents;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using Type = System.Type;

namespace Asset_Management_System.Models
{
    public abstract class Model
    {
        public ulong ID { get; protected set; }

        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
    }
}
