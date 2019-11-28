using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using Asset_Management_System.Logging;
using Asset_Management_System.Database.Repositories;

namespace Asset_Management_System.Models
{
    [Serializable]
    public class Asset : DoesContainFields, ILoggable<Asset>
    {
        public Asset() : base()
        {
            Description = "";
            Identifier = "";
        }

        [JsonConstructor]
        private Asset(ulong id, string name, string description, string identifier, ulong department_id, string options,
            DateTime created_at, DateTime updated_at)
        {
            ID = id;
            Name = name;
            Description = description;
            Identifier = identifier;
            DepartmentID = department_id;
            CreatedAt = created_at;
            SerializedFields = options;
            this.DeserializeFields();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Identifier { get; set; }

        public ulong DepartmentID { get; set; }

        /// <summary>
        /// Checks if the asset is equal to another asset based on
        /// the length of FieldsList, Name, Description, and DepartmentID
        /// </summary>
        /// <param name="obj">The object to compare the asset to</param>
        /// <returns>Rather the two objects are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is Asset == false)
            {
                return false;
            }

            Asset other = (Asset) obj;

            if (this.FieldsList.Count != other.FieldsList.Count ||
                this.Name != other.Name ||
                this.Description != other.Description ||
                this.DepartmentID != other.DepartmentID)
            {
                return false;
            }

            for (int i = this.FieldsList.Count - 1; i >= 0; i--)
            {
                if (!this.FieldsList[i].Equals(other.FieldsList[i]))
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
        public override int GetHashCode() => HashCode.Combine(Name, Description, DepartmentID);

        /// <summary>
        /// Returns the name of the asset
        /// </summary>
        /// <returns>Name of the asset</returns>
        public override string ToString() => Name;

        /// <summary>
        /// Saves all properties to a dictionary with Property name and value
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetLoggableProperties()
        {
            Dictionary<string, string> props = new Dictionary<string, string>();
            props.Add("ID", ID.ToString());
            props.Add("Name", Name);
            props.Add("Description", Description);
            props.Add("Department ID", DepartmentID.ToString());
            SerializeFields();
            props.Add("Options", SerializedFields);
            props.Add("Created at", DateToStringConverter);
            return props;

            /* Possible alternative using reflection
            PropertyInfo[] Props = this.GetType().GetProperties();
            foreach (var prop in Props)
            {
                props.Add(prop.Name, Props.GetValue(0).ToString());
            }
            return props
            /**/
        }

        /// <summary>
        /// Returns the Name that should be written in the log
        /// </summary>
        /// <returns></returns>
        public string GetLoggableName() => Name;

        /// <summary>
        /// Returns the ID
        /// </summary>
        /// <returns></returns>
        public ulong GetId() => ID;

        /// <summary>
        /// Returns a repository-instance for this class
        /// </summary>
        /// <returns></returns>
        public IRepository<Asset> GetRepository() => new AssetRepository();
    }
}