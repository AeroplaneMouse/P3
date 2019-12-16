using System;
using AMS.Models;
using System.Text;
using System.Collections.Generic;
using AMS.Controllers.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using AMS.Database.Repositories;
using AMS.Database.Repositories.Interfaces;
using AMS.Helpers;
using AMS.Interfaces;

namespace AMS.Controllers
{
    public class TagListController : ITagListController
    {
        private ITagRepository _rep { get; set; }

        public List<Tag> TagsList { get; set; }
        
        public IUserListController UserListController { get; set; }

        public TagListController(ITagRepository tagRepository)
        {
            _rep = tagRepository;
            TagsList = new List<Tag>();
        }

        public void Remove(Tag tag)
        {
            if (TagsList.Contains(tag) && tag.ID != 1)
            {
                TagsList.Remove(tag);
                _rep.Delete(tag);
            }
        }

        /// <summary>
        /// As the taglist only contains partial information on a tag, this function returns the complete tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Tag GetTag(ulong tagID)
        {
            return _rep.GetById(tagID);
        }

        /// <summary>
        /// Get the list of parent tags
        /// </summary>
        /// <returns></returns>
        public List<Tag> GetParentTags()
        {
            return _rep.GetParentTags().ToList();
        }
        
        /// <summary>
        /// Gets the child tags of an input parent tag
        /// </summary>
        /// <param name="id">ID of the parent tag</param>
        /// <returns></returns>
        public List<Tag> GetChildTags(ulong id)
        {
            return _rep.GetChildTags(id).ToList();
        }

        /// <summary>
        /// Gets all the tags stored in the system
        /// </summary>
        /// <param name="keyword"></param>
        public void GetTreeviewData(string keyword="")
        {
            TagsList = _rep.GetTreeViewDataList(keyword).ToList();
        }
    }
}
