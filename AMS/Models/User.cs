﻿using AMS.Database.Repositories;
using AMS.Database.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMS.Interfaces;
using System.Windows.Media;

namespace AMS.Models
{
    public class User : Model, ITagable
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
        public ulong DefaultDepartment { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }
        public string Domain { get; set; }

        private List<Department> _departmentList { get; set; }
        private IDepartmentRepository _departmentRep { get; set; }
        private ITagRepository _tagRepository { get; set; }

        // Index of the default department in the list of departments
        public int DepartmentIndex
        {
            get
            {
                if (_departmentList == null)
                {
                    _departmentList = new List<Department>() { new Department() { Name = "All departments" } };

                    // TODO: Ingen new repositories!
                    _departmentList.AddRange((_departmentRep ?? new DepartmentRepository()).GetAll().ToList());
                }

                return DefaultDepartment == 0 ? 0 : _departmentList.Select(p => p.ID).ToList().IndexOf(DefaultDepartment);
            }

            set
            {
                if (_departmentList == null)
                {
                    _departmentList = new List<Department>() { new Department() { Name = "All departments" } };

                    // TODO: Ingen new repositories!
                    _departmentList.AddRange((_departmentRep ?? new DepartmentRepository()).GetAll().ToList());
                }

                DefaultDepartment = (value == 0) ? 0 : _departmentList[value].ID;
            }
        }

        #region ITagable

        public ulong TagId => ID;
        public Type TagType => this.GetType();
        public string TagLabel => Username;
        public ulong ParentId => 1;
        public int ChildrenCount => 0;
        public List<ITagable> Children { get; set; }
        public string TagColor
        {
            // TODO: Ingen new repositories!
            get => (_tagRepository ?? new TagRepository()).GetById(1).TagColor;
            set => TagColor = value;
        }
        public SolidColorBrush TagFontColor => Notification.GetForegroundColor(TagColor);

        #endregion

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

        public User() { }

        public override string ToString() => Username;

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is UserWithStatus)
            {
                return base.Equals(obj);
            }

            ITagable objAsPart = obj as ITagable;

            if (objAsPart == null)
                return false;

            return ID == objAsPart.TagId 
                   && ParentId == objAsPart.ParentId;
        }
    }
}
