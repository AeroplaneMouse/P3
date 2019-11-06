using System;

namespace Asset_Management_System.Models
{
    public class User : Model, ITagable
    {
        /* Constructor used by DB */
        private User(ulong id, string name, string username, bool is_admin, ulong defaultDepartment, DateTime createdAt, DateTime updated_at)
        {
            ID = id;
            Name = name;
            Username = username;
            IsAdmin = is_admin;
            DefaultDepartment = defaultDepartment;
            CreatedAt = createdAt;
            UpdatedAt = updated_at;
        }
        
        public string Name { get; set; }

        public string Username { get; set; }

        public bool IsAdmin { get; set; }

        public ulong DefaultDepartment { get; set; }
        
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
