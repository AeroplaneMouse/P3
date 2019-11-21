using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace AMS.Models
{
    public class Asset : FieldContainer
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string Identifier { get; set; }

        public ulong DepartmentID { get; set; }

        public Asset()
        {
            FieldList = new ObservableCollection<Field>();
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
            Identifier = identifier;
            FieldList = JsonConvert.DeserializeObject<ObservableCollection<Field>>(SerializedFields);
            
        }
        
        private Asset(ulong id, string name, string description, string identifier, ulong departmentId, string options,
            DateTime created_at, DateTime updated_at)
        {
            ID = id;
            Name = name;
            Description = description;
            DepartmentID = departmentId;
            Identifier = identifier;
            SerializedFields = options;
            FieldList = JsonConvert.DeserializeObject<ObservableCollection<Field>>(SerializedFields);
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

        public override bool Equals(object obj)
        {
            if(obj is Asset == false)
            {
                return false;
            }

            Asset other = (Asset)obj;

            if (this.FieldList.Count != other.FieldList.Count ||
                this.Name != other.Name ||
                this.Description != other.Description ||
                this.DepartmentID != other.DepartmentID)
            {
                return false;
            }

            for(int i = this.FieldList.Count - 1; i >= 0; i--)
            {
                if(! this.FieldList[i].Equals(other.FieldList[i]))
                {
                    return false;
                }
            }

            return true;
        }

    }
}