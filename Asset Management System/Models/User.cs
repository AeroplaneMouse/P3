﻿using Asset_Management_System.Database.Repositories;
using System;

namespace Asset_Management_System.Models
{
    public class User : Model, ITagable
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

        public bool Equals(User other)
        {
            Console.WriteLine("Test");
            return other != null && this.Username.CompareTo(other.Username) == 0 && this.ID.CompareTo(other.ID) == 0;
        }

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

    public class UserWithStatus : User
    {
        public UserWithStatus(User user) : base()
        {
            this.ID = user.ID;
            this.Username = user.Username;
            this.Domain = user.Domain;
            this.Description = user.Description;
            this.IsEnabled = user.IsEnabled;
            this.DefaultDepartment = user.DefaultDepartment;
            this.IsAdmin = user.IsAdmin;
            this.CreatedAt = user.CreatedAt;
            this.UpdatedAt = user.UpdatedAt;

            this.Status = String.Empty;
            IsShown = true;
        }

        public string Status { get; set; }

        public string StatusColor
        {
            get
            {
                if (Status.CompareTo("Added") == 0)
                {
                    return "#00B600";
                }

                else if (Status.CompareTo("Removed") == 0)
                {
                    return "#E30000";
                }

                else if (Status.CompareTo("Conflict") == 0)
                {
                    return "#FFCC1A";
                }

                else if (IsEnabled == false)
                {
                    return "#C0C0C0";
                }

                else
                {
                    return "#ffffff";
                }
            }
        }

        public bool IsShown { get; set; }
    }
}
