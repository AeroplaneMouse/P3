using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Logging;
using AMS.Models;
using AMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AMS.Controllers
{
    public class TagController : FieldListController, ITagController
    {
        private ITagRepository _tagRepository { get; set; }
        private IDepartmentRepository _departmentRepository { get; set; }
        private Tag _controlledTag;

        public Tag ControlledTag 
        { 
            get => _controlledTag;
            set
            {
                _controlledTag = value;
                UpdateParameters(value);
            }
        }

        public bool IsEditing { get; set; }
        public ulong TagID;

        public ulong Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public ulong ParentID { get; set; }
        public ulong DepartmentID { get; set; }


        public List<Tag> ParentTagList
        {
            get
            {
                List<Tag> parentTagsList = new List<Tag>()
                {
                    new Tag()
                    {
                        Name = "[No Parent Tag]",
                        ParentID = 0,
                        TagColor = ControlledTag.TagColor
                    }
                };
                foreach (Tag parentTag in (List<Tag>) _tagRepository.GetParentTags()
                    .Where(t => t.ID != 1 && t.ID != ControlledTag.ID)
                    .ToList())
                {
                    parentTagsList.Add(parentTag);
                }

                return parentTagsList;
            }
        }
        
        public List<Department> DepartmentList
        {
            get
            {
                List<Department> departments = new List<Department>()
                {
                    new Department() { Name = "All departments" }
                };

                _departmentRepository.GetAll().ToList().ForEach(d => departments.Add(d));

                return departments;
            }
        }

        public TagController(Tag tag, ITagRepository tagRep, IDepartmentRepository departmentRepository) 
            : base(tag)
        {
            ControlledTag = tag;
            _tagRepository = tagRep;
            _departmentRepository = departmentRepository;
        }

        private void UpdateParameters(Tag value)
        {
            if (value != null && value.ID != 0)
            {
                IsEditing = true;
                ControlledTag.DeSerializeFields();

                Id = ControlledTag.ID;
                Name = ControlledTag.Name;
                Color = ControlledTag.Color;
                ParentID = ControlledTag.ParentID;
                DepartmentID = ControlledTag.DepartmentID;
            }
            else
            {
                _controlledTag = new Tag { Color = CreateRandomColor() };
                Color = _controlledTag.Color;
                _controlledTag.FieldList = new List<Field>();
                IsEditing = false;
            }
            NonHiddenFieldList = ControlledTag.FieldList.Where(f => f.IsHidden == false).ToList();
            HiddenFieldList = ControlledTag.FieldList.Where(f => f.IsHidden == true).ToList();
        }

        #region Public Methods

        /// <summary>
        /// Saves the tag.
        /// </summary>
        public void Save()
        {
            //Updates the fields on the tag
            ControlledTag.Name = Name;
            ControlledTag.ParentID = ParentID;
            ControlledTag.DepartmentID = (ParentID != 0 ? _tagRepository.GetById(ParentID).DepartmentID : DepartmentID);
            ControlledTag.Color = Color;

            List<Field> fieldList = NonHiddenFieldList;
            fieldList.AddRange(HiddenFieldList);
            ControlledTag.FieldList = fieldList;
            SerializeFields();
            _tagRepository.Insert(ControlledTag, out TagID);
        }

        /// <summary>
        /// Updates the tag.
        /// </summary>
        public void Update()
        {
            //Updates the fields on the tag
            if (ControlledTag.Name != Name)
            {
                ControlledTag.Name = Name;
            }

            if (ControlledTag.ParentID != ParentID)
            {
                ControlledTag.ParentID = ParentID;

                if (ControlledTag.DepartmentID != DepartmentID)
                {
                    ControlledTag.DepartmentID = (ParentID != 0 ? _tagRepository.GetById(ParentID).DepartmentID : DepartmentID);
                }
            }

            if (ControlledTag.Color != Color)
            {
                ControlledTag.Color = Color;
            }

            List<Field> fieldList = NonHiddenFieldList;
            fieldList.AddRange(HiddenFieldList);
            ControlledTag.FieldList = fieldList;
            SerializeFields();
            _tagRepository.Update(ControlledTag);
        }

        public void Remove() => _tagRepository.Delete(ControlledTag);

        public void RemoveChildren() => _tagRepository.DeleteChildren(ControlledTag.ID);

        public string CreateRandomColor()
        {
            //Creates an instance of the Random, to create pseudo random numbers
            Random random = new Random();

            //Creates a hex values from three random ints converted to bytes and then to string
            string hex = "#" + ((byte) random.Next(100, 230)).ToString("X2") +
                               ((byte) random.Next(100, 230)).ToString("X2") + 
                               ((byte) random.Next(100, 230)).ToString("X2");

            return hex;
        }
        
        /// <summary>
        /// Connects a parentTag, and removes the relation to the old parent tag.
        /// </summary>
        /// <param name="newTag"></param>
        /// <param name="oldTag"></param>
        public void ConnectTag(Tag newTag, Tag oldTag)
        {
            //Adds the fields
            foreach (var field in newTag.FieldList)
            {
                AddField(field, newTag);
            }

            //Removes the fields needed to be removed (Both in hidden and non hidden list)
            List<Field> fieldsToRemove = new List<Field>();
            foreach (var field in NonHiddenFieldList)
            {
                if (field.TagIDs.Count == 1 && field.TagIDs.Contains(oldTag.ID))
                {
                    fieldsToRemove.Add(field);
                }
            }
            foreach (var field in HiddenFieldList)
            {
                if (field.TagIDs.Count == 1 && field.TagIDs.Contains(oldTag.ID))
                {
                    fieldsToRemove.Add(field);
                }
            }

            foreach (var field in fieldsToRemove)
            {
                RemoveField(field);
            }
            
        }
        #endregion
    }
}