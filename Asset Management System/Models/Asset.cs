using System;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;

namespace Asset_Management_System.Models
{
    [Serializable]
    public class Asset : DoesContainFields
    {
        public Asset()
        {
            FieldsList = new List<Field>();
        }

        [JsonConstructor]
        private Asset(ulong id, string name, string description, ulong department_id, DateTime created_at, string options)
        {
            ID = id;
            Name = name;
            Description = description;
            DepartmentID = department_id;
            CreatedAt = created_at;
            SerializedFields = options;
            this.DeserializeFields();
            SavePrevValues();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public ulong DepartmentID { get; set; }

        public override string ToString() => Name;
    }
}