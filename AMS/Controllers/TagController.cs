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
    public class TagController : FieldListController, ITagController, IFieldListController
    {
        private ITagRepository _tagRepository { get; set; }
        private IDepartmentRepository _departmentRepository { get; set; }


        public Tag Tag { get; set; }
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
                        TagColor = Tag.TagColor
                    }
                };
                foreach (Tag parentTag in (List<Tag>) _tagRepository.GetParentTags()
                    .Where(t => t.ID != 1 && t.ID != Tag.ID)
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
                    new Department()
                    {
                        Name = "All departments"
                    }
                };

                _departmentRepository.GetAll().ToList().ForEach(d => departments.Add(d));

                return departments;
            }
        }

        public TagController(Tag tag, ITagRepository tagRep, IDepartmentRepository departmentRepository) : base(tag ?? new Tag())
        {
            Tag = tag ?? new Tag();
            _tagRepository = tagRep;
            _departmentRepository = departmentRepository;

            if (Tag.ID != 0)
            {
                IsEditing = true;
                Tag.DeSerializeFields();

                Id = tag.ID;
                Name = tag.Name;
                Color = tag.TagColor;
                ParentID = tag.ParentID;
                DepartmentID = tag.ParentID;

                NonHiddenFieldList = tag.FieldList.Where(f => f.IsHidden == false).ToList();
                HiddenFieldList = tag.FieldList.Where(f => f.IsHidden == true).ToList();
            }
            else
            {
                Tag = new Tag {TagColor = CreateRandomColor()};
                IsEditing = false;
            }

            NonHiddenFieldList = Tag.FieldList.Where(f => f.IsHidden == false).ToList();
            HiddenFieldList = Tag.FieldList.Where(f => f.IsHidden == true).ToList();
        }

        #region Public Methods

        public void Save()
        {
            List<Field> fieldList = NonHiddenFieldList;
            fieldList.AddRange(HiddenFieldList);
            Tag.FieldList = fieldList;
            SerializeFields();
            _tagRepository.Insert(Tag, out TagID);
        }

        public void Update()
        {
            List<Field> fieldList = NonHiddenFieldList;
            fieldList.AddRange(HiddenFieldList);
            Tag.FieldList = fieldList;
            SerializeFields();
            _tagRepository.Update(Tag);
        }

        public void Remove() => _tagRepository.Delete(Tag);

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

        public void ConnectTag(Tag newTag, Tag oldTag)
        {
            foreach (var field in newTag.FieldList)
            {
                AddField(field, newTag);
            }

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