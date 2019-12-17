using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AMS.Logging;
using Newtonsoft.Json;

namespace AMS.Models
{
    public class Asset : FieldContainer
    {
        private string _name;
        private string _description;
        private string _identifier;
        private ulong _departmentId;

        public string Name
        {
            get => _name;
            set
            {
                string propertyName = "Name";

                if (TrackChanges && !Changes.ContainsKey(propertyName) && _name != value)
                    Changes[propertyName] = _name;

                else if (Changes.ContainsKey(propertyName) && (string)Changes[propertyName] == value)
                    Changes.Remove(propertyName);

                _name = value;
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                string propertyName = "Description";

                if (TrackChanges && !Changes.ContainsKey(propertyName) && _description != value)
                    Changes[propertyName] = _description;

                else if (Changes.ContainsKey(propertyName) && (string)Changes[propertyName] == value)
                    Changes.Remove(propertyName);

                _description = value;
            }
        }

        public string Identifier
        {
            get => _identifier;
            set
            {
                string propertyName = "Identifier";

                if (TrackChanges && !Changes.ContainsKey(propertyName) && _identifier != value)
                    Changes[propertyName] = _identifier;

                else if (Changes.ContainsKey(propertyName) && (string)Changes[propertyName] == value)
                    Changes.Remove(propertyName);

                _identifier = value;
            }
        }

        public ulong DepartmentdId
        {
            get => _departmentId;
            set
            {
                string propertyName = "DepartmentId";

                if (TrackChanges && !Changes.ContainsKey(propertyName) && _departmentId != value)
                    Changes[propertyName] = _departmentId;

                else if (Changes.ContainsKey(propertyName) && (ulong)Changes[propertyName] == value)
                    Changes.Remove(propertyName);

                _departmentId = value;

            }
        }

        public string AssociatedTags { get; set; } = "";
        public string AssociatedUsers { get; set; } = "";

        public Asset()
        {
            FieldList = new List<Field>();
        }

        [JsonConstructor]
        private Asset(ulong id, string name, string description, string identifier, ulong departmentId,
            string serializedFields,
            DateTime created_at, DateTime updated_at, string tags, string users)
        {
            ID = id;
            Name = name;
            Description = description;
            DepartmentdId = departmentId;
            Identifier = identifier;
            SerializedFields = serializedFields;
            CreatedAt = created_at;
            UpdatedAt = updated_at;
            AssociatedTags = tags;
            AssociatedUsers = users;
            TrackChanges = true;
        }

        /// <summary>
        /// Creates hash code based on Name, Description, and DepartmentdId
        /// </summary>
        /// <returns>The calculated hash code</returns>
        public override int GetHashCode() => HashCode.Combine(Name, Description, DepartmentdId);

        /// <summary>
        /// Returns the name of the asset
        /// </summary>
        /// <returns>Name of the asset</returns>
        public override string ToString() => Name;

        public override bool Equals(object obj) => obj is Asset other && ID == other.ID;

        public bool DeSerializeFields()
        {
            if (!string.IsNullOrEmpty(SerializedFields))
            {
                FieldList = JsonConvert.DeserializeObject<List<Field>>(SerializedFields);
                return true;
            }

            return false;
        }
    }
}