using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Logging;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using Type = System.Type;

namespace Asset_Management_System.Models
{
    [Serializable]
    public class Asset : DoesContainFields, ILoggable<Asset>
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
