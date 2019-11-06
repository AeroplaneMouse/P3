using System;
using System.Collections.Generic;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Logging;
using Google.Protobuf.WellKnownTypes;

namespace Asset_Management_System.Models
{
    public class Tag : DoesContainFields, ILoggable<Tag>, ITagable
    {
        public Tag()
        {
            
        }

        /*Constructor used by DB*/
        private Tag(ulong id, string name, ulong department_id, ulong parent_id, string color, DateTime created_at, DateTime updated_at,string SerializedField)
        {
            ID = id;
            Name = name;
            DepartmentID = department_id;
            ParentID = parent_id;
            Color = color;
            this.SerializedFields = SerializedField;
            CreatedAt = created_at;
            UpdatedAt = updated_at;
            DeserializeFields();
        }

        public string Name { get; set; }

        public string Color { get; set; }

        public ulong ParentID { get; set; }

        public ulong DepartmentID { get; set; }

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
            props.Add("Department ID", DepartmentID.ToString());
            props.Add("Parent ID", ParentID.ToString());
            props.Add("Color", Color);
            props.Add("Options", SerializedFields);
            props.Add("Created at", DateToStringConverter);
            return props;
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
        public IRepository<Tag> GetRepository() => new TagRepository();

        public ulong TagId()
        {
            return ID;
        }

        public string TagType()
        {
            return this.GetType().ToString();
        }

        public string TagLabel()
        {
            return Name;
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null) 
                return false;
            
            ITagable objAsPart = obj as ITagable;
            
            if (objAsPart == null) 
                return false;
            
            return ID.Equals(objAsPart.TagId());
        }

        public bool Equals(Tag other)
        {
            return other != null && ID.Equals(other.ID);
        }
    }
}