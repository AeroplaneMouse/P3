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

        public string Name {
            get => this._name;
            set {
                if (this.Name != null)
                {
                    this.Changes["Name"] = this.Name;
                }
                this._name = value;
            }
        }
        public string Description {
            get => this._description;
            set {
                if (this.Description != null)
                {
                    this.Changes["Description"] = this.Description;
                }
                this._description = value;
            }
        }

        public string Identifier {
            get => this._identifier;
            set {
                if (this.Identifier != null)
                {
                    this.Changes["Identifier"] = this.Identifier;
                }
                this._identifier = value;
            }
        }

        public ulong DepartmentID {
            get {
                return this._departmentID;
            }
            set {
                if (this.DepartmentID > 0)
                {
                    this.Changes["DepartmentID"] = this.DepartmentID;
                }
                this._departmentID = value;
            }
        }

        public Asset()
        {
            FieldList = new List<Field>();
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
            Asset other = obj as Asset;

            if(other == null)
                return false;

            if (ID != other.ID)
                return false;

            for(int i = this.FieldList.Count - 1; i >= 0; i--)
            {
                if(!this.FieldList[i].Equals(other.FieldList[i]))
                    return false;
            }

            return true;
        }
        
        public bool DeSerializeFields()
        {
            if (!string.IsNullOrEmpty(this.SerializedFields))
            {
                this.FieldList =
                    JsonConvert.DeserializeObject<List<Field>>(this.SerializedFields);
                return true;
            }

            return false;
        }

    }
}