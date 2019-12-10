using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace AMS.Models
{
    public class Asset : FieldContainer
    {
        private string _name;
        private string _description;
        private string _identifier;
        private ulong _departmentID;

        public string Name
        {
            get => _name;
            set
            {
                if (TrackChanges)
                {
                    Changes["Name"] = Name;
                }

                _name = value;
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (TrackChanges)
                {
                    Changes["Description"] = Description;
                }

                _description = value;
            }
        }

        public string Identifier
        {
            get => _identifier;
            set
            {
                if (TrackChanges)
                {
                    Changes["Identifier"] = Identifier;
                }

                _identifier = value;
            }
        }

        public ulong DepartmentID
        {
            get => _departmentID;
            set
            {
                if (TrackChanges)
                {
                    Changes["DepartmentID"] = DepartmentID;
                }

                _departmentID = value;
            }
        }

        public Asset()
        {
            FieldList = new List<Field>();
        }

        [JsonConstructor]
        private Asset(ulong id, string name, string description, string identifier, ulong departmentId,
            string serializedFields,
            DateTime created_at, DateTime updated_at)
        {
            ID = id;
            Name = name;
            Description = description;
            DepartmentID = departmentId;
            Identifier = identifier;
            SerializedFields = serializedFields;
            CreatedAt = created_at;
            UpdatedAt = updated_at;
            TrackChanges = true;
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
            return obj is Asset other && ID == other.ID;
        }

        public bool DeSerializeFields()
        {
            if (!string.IsNullOrEmpty(this.SerializedFields))
            {
                this.FieldList = JsonConvert.DeserializeObject<List<Field>>(this.SerializedFields);
                return true;
            }

            return false;
        }
    }
}