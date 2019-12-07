using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Logging;
using AMS.Models;
using AMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.RightsManagement;

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
            set => _controlledTag = value;
        }

        public List<Field> ParentTagFields { get; set; } = new List<Field>();
        public bool IsEditing { get; set; }
        public ulong TagID;
        public ulong Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public ulong ParentId { get; set; }
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
            _controlledTag = tag;

            _tagRepository = tagRep;
            _departmentRepository = departmentRepository;

            if (_controlledTag.ID == 0)
            {
                _controlledTag.Color = CreateRandomColor();
                Color = _controlledTag.Color;
                IsEditing = false;
            }
            else
            {
                IsEditing = true;
                _controlledTag.DeSerializeFields();
            }
            
            
            Id = _controlledTag.ID;
            Name = _controlledTag.Name;
            Color = _controlledTag.Color;
            ParentId = _controlledTag.ParentId;
            DepartmentID = _controlledTag.DepartmentID;

            NonHiddenFieldList = _controlledTag.FieldList.Where(f => f.IsHidden == false).ToList();
            HiddenFieldList = _controlledTag.FieldList.Where(f => f.IsHidden == true).ToList();
        }

        #region Public Methods

        /// <summary>
        /// Saves the tag.
        /// </summary>
        public void Save()
        {
            //Updates the fields on the tag
            if (Name != _controlledTag.Name)
                _controlledTag.Name = Name;
            if (ParentId != _controlledTag.ParentId)
                _controlledTag.ParentId = ParentId;

            _controlledTag.DepartmentID = (ParentId != 0 ? _tagRepository.GetById(ParentId).DepartmentID : DepartmentID);
            if (Color != _controlledTag.Color)
                _controlledTag.Color = Color;


            List<Field> fieldList = NonHiddenFieldList;
            fieldList.AddRange(HiddenFieldList);
            _controlledTag.FieldList = fieldList;
            SerializeFields();

            _tagRepository.Insert(ControlledTag, out TagID);
        }

        /// <summary>
        /// Updates the tag.
        /// </summary>
        public void Update()
        {
            //Updates the fields on the tag
            if (_controlledTag.Name != Name)
            {
                _controlledTag.Name = Name;
            }

            if (_controlledTag.ParentId != ParentId)
            {
                _controlledTag.ParentId = ParentId;

                if (_controlledTag.DepartmentID != DepartmentID)
                {
                    ControlledTag.DepartmentID =
                        (ParentId != 0 ? _tagRepository.GetById(ParentId).DepartmentID : DepartmentID);
                }
            }

            if (_controlledTag.Color != Color)
            {
                _controlledTag.Color = Color;
            }

            List<Field> fieldList = NonHiddenFieldList;
            fieldList.AddRange(HiddenFieldList);
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
            //TODO Throws exception, når et tags parent id ændres
            currentTag.DeSerializeFields();
            ParentTagFields = currentTag.FieldList;
        }

        #endregion

        /// <summary>
        /// Reverts the changes made to the TagController, to correspond with the information on the tag.
        /// </summary>
        public void RevertChanges()
        {
            _controlledTag = _tagRepository.GetById(_controlledTag.ID);
        }
    }
}