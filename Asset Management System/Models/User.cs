using Asset_Management_System.Database.Repositories;
using System;

namespace Asset_Management_System.Models
{
    public class User : Model
    {
        /* Constructor used by DB */
        private User(ulong id, string username, string domain, string description, bool is_enabled, ulong defaultDepartment, bool is_admin, DateTime createdAt, DateTime updated_at)
        {
            ID = id;
            Username = username;
            Domain = domain;
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

        public string Username { get; set; }

        public bool IsAdmin { get; set; }

        public ulong DefaultDepartment { get; set; }

        public string DefaultDepartmentName
        {
            get
            {
                if (DefaultDepartment == 0)
                {
                    return "All Departments";
                }

                var department = new DepartmentRepository().GetById(DefaultDepartment);

                return department == null ? String.Empty : department.Name;
            }
        }

        public string Description { get; set; }

        public bool IsEnabled { get; set; }

        public string Domain { get; set; }
    }
}
