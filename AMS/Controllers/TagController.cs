using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Models;
using AMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Controllers
{
    public class TagController : FieldListController, ITagController
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
                        Color = Tag.Color
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
                Tag.Color = CreateRandomColor();
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
    }
}
