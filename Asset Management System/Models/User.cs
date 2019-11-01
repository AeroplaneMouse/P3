using System;

namespace Asset_Management_System.Models
{
    public class User : Model
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

        public User()
        {

        }
        
        public string Name { get; set; }

        public string Username { get; set; }

        public bool IsAdmin { get; set; }

        public ulong DefaultDepartment { get; set; }

        public string Description { get; set; }
    }
}
