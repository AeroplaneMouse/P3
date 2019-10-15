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

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Description, DepartmentID);
        }

        public override string ToString() => Name;
    }
}