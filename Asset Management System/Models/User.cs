using System;

namespace Asset_Management_System.Models
{
    public class User : Model
    {
        /* Constructor used by DB */
        private User(ulong id, string name, string username, bool is_admin, DateTime createdAt)
        {
            ID = id;
            Name = name;
            Username = username;
            IsAdmin = is_admin;
            CreatedAt = createdAt;
        }
        
        public string Name { get; set; }

        public string Username { get; set; }

        public bool IsAdmin { get; set; }
    }
}
