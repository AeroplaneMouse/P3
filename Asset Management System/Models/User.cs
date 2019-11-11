using System;
using Asset_Management_System.Database.Repositories;

namespace Asset_Management_System.Models
{
    public class User : Model, ITagable
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
        public ulong DefaultDepartment { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }
        public string DefaultDepartmentName
        {
            get
            {
                if (DefaultDepartment == 0)
                    return "All departments";

                var department = new DepartmentRepository().GetById(DefaultDepartment);

                return department == null ? String.Empty : department.Name;
            }
        }

        /* Constructor used by DB */
        private User(ulong id, string name, string username, string description, bool is_enabled, ulong defaultDepartment, bool is_admin, DateTime createdAt, DateTime updated_at)
        {
            ID = id;
            Name = name;
            Username = username;
            Description = description;
            IsEnabled = is_enabled;
            DefaultDepartment = defaultDepartment;
            IsAdmin = is_admin;
            CreatedAt = createdAt;
            UpdatedAt = updated_at;
        }

        public User()
        {

        }

        public ulong TagId()
        {
            return ID;
        }

        public string TagType()
        {
            return GetType().ToString();
        }

        public string TagLabel()
        {
            return Username;
        }

        public override string ToString() 
        {
            return Username;
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
