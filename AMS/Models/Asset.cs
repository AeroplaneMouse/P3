using System;
using Newtonsoft.Json;

namespace AMS.Models
{
    public class Asset : ContainsFields
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string Identifier { get; set; }

        public ulong DepartmentID { get; set; }

        public Asset()
        {
        }

        [JsonConstructor]
        private Asset(ulong id, string name, string description, string identifier, ulong departmentId, string options,
            DateTime created_at, DateTime updated_at, string serializedFields)
        {
            ID = id;
            Name = name;
            Description = description;
            DepartmentID = departmentId;
            SerializedFields = serializedFields;
            CreatedAt = created_at;
            UpdatedAt = updated_at;
        }

        /// <summary>
        /// Creates hash code based on Name, Description, and DepartmentID
        /// </summary>
        /// <returns>The calculated hash code</returns>
        public override int GetHashCode() => HashCode.Combine(Name, Description, DepartmentID);

        /// <summary>
        /// Returns the name of the asset
        /// </summary>
        /// <returns>Name of the asset</returns>
        public override string ToString() => Name;
    }
}