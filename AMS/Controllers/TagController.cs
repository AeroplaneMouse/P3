using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Logging;
using AMS.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AMS.Controllers
{
    public class TagController : FieldListController, ITagController, ILoggableValues, IFieldListController
    {
        public Tag Tag { get; set; }
        public bool IsEditing { get; set; }
        public ulong TagID;

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



        public TagController(Tag tag, ITagRepository tagRep) :  base(tag ?? new Tag())
        {
            Tag = tag;
            _tagRepository = tagRep;

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

            NonHiddenFieldList = Tag.FieldList.Where(f => f.IsHidden == false).ToList();
            HiddenFieldList = Tag.FieldList.Where(f => f.IsHidden == true).ToList();
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

        /// <summary>
        /// Makes a loggable dictionary from the tag
        /// </summary>
        /// <returns>The tag formatted as a loggable dictionary</returns>
        public Dictionary<string, string> GetLoggableValues()
        {
            Dictionary<string, string> props = new Dictionary<string, string>();
            props.Add("ID", Tag.ID.ToString());
            props.Add("Name", Tag.Name);
            props.Add("ParentId", Tag.ParentID.ToString());
            props.Add("Number of children", Tag.NumOfChildren.ToString());
            props.Add("Department ID", Tag.DepartmentID.ToString());
            SerializeFields();
            props.Add("Created at", Tag.CreatedAt.ToString());
            props.Add("Last updated at", Tag.UpdatedAt.ToString());

            return props;
        }

        /// <summary>
        /// Returns the name of the tag
        /// </summary>
        /// <returns>The name of the tag</returns>
        public string GetLoggableTypeName() => Tag.Name;

        #endregion
    }
}