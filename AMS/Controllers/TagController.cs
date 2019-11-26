using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Logging;
using AMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AMS.Controllers
{
    public class TagController : FieldListController, ITagController, IFieldListController
    {
        public Tag Tag { get; set; }
        public bool IsEditing { get; set; }
        public ulong TagID;

        public string Name { get; set; }
        public string Color { get; set; }
        public ulong ParentID { get; set; }
        public ulong DepartmentID { get; set; }

        #region Private Properties

        private ITagRepository _tagRepository { get; set; }

        #endregion

        #region Constructor


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
                foreach (Tag parentTag in (List<Tag>) _tagRepository.GetParentTags())
                    parentTagsList.Add(parentTag);

                return parentTagsList;
            }
        }

        public TagController(Tag tag, ITagRepository tagRep) : base(tag)
        {
            Tag = tag;
            _tagRepository = tagRep;


            Name = tag.Name;
            Color = tag.TagColor;
            ParentID = tag.ParentID;
            DepartmentID = tag.ParentID;

        NonHiddenFieldList = tag.FieldList.Where(f => f.IsHidden == false).ToList();
            HiddenFieldList = tag.FieldList.Where(f => f.IsHidden == true).ToList();

            if (Tag != null)
            {
                IsEditing = true;
            }
            else
            {
                Tag = new Tag();
                Tag.TagColor = CreateRandomColor();
                IsEditing = false;
            }
        }

        #endregion

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
            string hex = "#" + ((byte) random.Next(25, 230)).ToString("X2") +
                         ((byte) random.Next(25, 230)).ToString("X2") + ((byte) random.Next(25, 230)).ToString("X2");

            return hex;
        }

        public void ConnectTag(Tag newTag, Tag oldTag)
        {
            foreach (var field in newTag.FieldList)
            {
                AddField(field, newTag);
                Console.WriteLine("Added field " + field.Label);
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