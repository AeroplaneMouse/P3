using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Logging;
using AMS.Models;
using AMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.RightsManagement;

namespace AMS.Controllers
{
    public class TagController : FieldListController, ITagController
    {
        private ITagRepository _tagRepository { get; set; }
        private IDepartmentRepository _departmentRepository { get; set; }

        public Tag ControlledTag
        {
            get => (Tag)_fieldContainer;
            set => _fieldContainer = value;
        }

        public List<Field> ParentTagFields { get; set; } = new List<Field>();
        public bool IsEditing { get; set; }

        public ulong TagID { get; set; }

        public List<Tag> ParentTagList
        {
            get
            {
                List<Tag> parentTagsList = new List<Tag>()
                {
                    new Tag()
                    {
                        Name = "[No Parent Tag]",
                        ParentId = 0,
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
                    new Department() {Name = "All departments"}
                };

                _departmentRepository.GetAll().ToList().ForEach(d => departments.Add(d));

                return departments;
            }
        }

        public TagController(Tag tag, ITagRepository tagRep, IDepartmentRepository departmentRepository)
            : base(tag)
        {
            //_controlledTag = tag;

            _tagRepository = tagRep;
            _departmentRepository = departmentRepository;

            if (ControlledTag.ID == 0)
            {
                ControlledTag.Color = CreateRandomColor();
                IsEditing = false;
            }
            else
            {
                IsEditing = true;
                ControlledTag.DeSerializeFields();
            }
        }

        #region Public Methods

        /// <summary>
        /// Saves the tag as a new tag in the database
        /// </summary>
        public void Save()
        {
            SerializeFields();
            ulong newTagId;
            ControlledTag = _tagRepository.Insert(ControlledTag, out newTagId);
            TagID = newTagId;

            // Check if any fields does not have a tagId set
            ControlledTag.DeSerializeFields();
            foreach(Field field in ControlledTag.FieldList)
            {
                if (field.TagIDs.Count == 0)
                {
                    if (field.TagIDs.Contains(ControlledTag.ID)) continue;
                    field.TagIDs.Add(ControlledTag.ID);
                    ControlledTag.Changes["options"] = TagID;
                }
            }

            // Save the field changes to the database
            if (ControlledTag.IsDirty())
                Update();
        }

        /// <summary>
        /// Updates the tag in the database
        /// </summary>
        public void Update()
        {
            SerializeFields();
            _tagRepository.Update(ControlledTag);
        }

        /// <summary>
        /// Removes the controlled tag from the repository, and optionally its children
        /// </summary>
        /// <param name="removeChildren">Rather or not to remove the children of the tag as well</param>
        /// <returns></returns>
        public bool Remove(bool removeChildren = false) => _tagRepository.Delete(ControlledTag, removeChildren);

        /// <summary>
        /// Returns a tag with the given tag, or null if tag does not exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Tag GetTagById(ulong id)
        {
            return _tagRepository.GetById(id);
        }

        /// <summary>
        /// Returns a random hex color string within a specified limit
        /// </summary>
        /// <returns></returns>
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
        public void ConnectParentTag()
        {
            Tag currentTag = _tagRepository.GetById(ControlledTag.ParentId);
            if (currentTag != null)
            {
                //TODO Throws exception, når et tags parent id ændres
                currentTag.DeSerializeFields();
                ParentTagFields = currentTag.FieldList;
            }
            else
                ParentTagFields = new List<Field>();
        }

        #endregion

        /// <summary>
        /// Reverts the changes made to the TagController, to correspond with the information on the tag.
        /// </summary>
        public void RevertChanges()
        {
            foreach (PropertyInfo property in ControlledTag.GetType().GetProperties())
            {
                if (ControlledTag.Changes.ContainsKey(property.Name))
                    property.SetValue(ControlledTag, ControlledTag.Changes[property.Name].ToString());
            }

            ControlledTag.Changes = new Dictionary<string, object>();
        }
    }
}