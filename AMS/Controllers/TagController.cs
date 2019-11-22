using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Logging;
using AMS.Models;
using System;
using System.Collections.Generic;

namespace AMS.Controllers
{
    public class TagController : FieldListController, ITagController, ILoggableValues
    {
        public Tag Tag { get; set; }
        public string PageTitle { get; set; }

        public ulong tagID;

        ITagRepository _tagRepository { get; set; }

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
                foreach (Tag parentTag in (List<Tag>)_tagRepository.GetParentTags())
                    parentTagsList.Add(parentTag);

                return parentTagsList;
            }
        }

        public TagController(Tag tag, ITagRepository tagRep) : base(tag)
        {
            Tag = tag;

            _tagRepository = tagRep;

            if (Tag != null)
            {
                PageTitle = "Edit tag";
            }
            else
            {
                Tag = new Tag();
                Tag.TagColor = CreateRandomColor();
                PageTitle = "Add tag";
            }
        }

        public void Save()
        {
            _tagRepository.Insert(Tag, out tagID);
        }

        public void Remove()
        {
            _tagRepository.Delete(Tag);
        }

        public void Update()
        {
            _tagRepository.Update(Tag);
        }

        public string CreateRandomColor()
        {
            //Creates an instance of the Random, to create pseudo random numbers
            Random random = new Random();

            //Creates a hex values from three random ints converted to bytes and then to string
            string hex = "#" + ((byte)random.Next(25, 230)).ToString("X2") +
                         ((byte)random.Next(25, 230)).ToString("X2") + ((byte)random.Next(25, 230)).ToString("X2");

            return hex;
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
    }
}
