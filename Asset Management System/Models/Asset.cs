using System;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;

namespace Asset_Management_System.Models
{
    [Serializable]
    public class Asset : DoesContainFields
    {
        public Asset() : base() { }

        [JsonConstructor]
        private Asset(ulong id, string name, string description, ulong department_id, string options, DateTime created_at, DateTime updated_at)
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

        /// <summary>
        /// Checks if the asset is equal to another asset based on
        /// the length of FieldsList, Name, Description, and DepartmentID
        /// </summary>
        /// <param name="obj">The object to compare the asset to</param>
        /// <returns>Rather the two objects are equal</returns>
        public override bool Equals(object obj)
        {
            if(obj is Asset == false)
            {
                return false;
            }

            Asset other = (Asset)obj;

            if (this.FieldsList.Count != other.FieldsList.Count ||
                this.Name != other.Name ||
                this.Description != other.Description ||
                this.DepartmentID != other.DepartmentID)
            {
                return false;
            }

            for(int i = this.FieldsList.Count - 1; i >= 0; i--)
            {
                if(! this.FieldsList[i].Equals(other.FieldsList[i]))
                {
                    return false;
                }
            }
            
            return true;
        }

        /// <summary>
        /// Creates hash code based on Name, Description, and DepartmentID
        /// </summary>
        /// <returns>The calculated hash code</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Description, DepartmentID);
        }

        /// <summary>
        /// Returns the name of the asset
        /// </summary>
        /// <returns>Name of the asset</returns>
        public override string ToString() => Name;
    }
}