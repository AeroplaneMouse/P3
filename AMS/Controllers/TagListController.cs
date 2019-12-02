﻿using System;
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
        private readonly ITagRepository _rep;
        private IExporter _exporter;

        public List<Tag> TagsList { get; set; }
        
        public IUserListController UserListController { get; set; }

        public TagListController(ITagRepository tagRepository, IExporter printHelper)
        {
            _rep = tagRepository;
            _exporter = printHelper;
            TagsList = new List<Tag>();
        }

        public void Search(string query)
        {
            TagsList = _rep.Search("").ToList();
        }

        public void Remove(Tag tag)
        {
            if (TagsList.Contains(tag) && tag.ID != 1)
            {
                TagsList.Remove(tag);
                _rep.Delete(tag);
            }
        }

        public void Export(List<Tag> tags)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// As the taglist only contains partial information on a tag, this function returns the complete tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Tag getTag(Tag tag)
        {
            return _rep.GetById(tag.ID);
        }

        public List<Tag> GetParentTags()
        {
            return _rep.GetParentTags().ToList();
        }
        
        public List<Tag> GetChildTags(ulong id)
        {
            return _rep.GetChildTags(id).ToList();
        }

        public void GetTreeviewData(string keyword="")
        {
            TagsList = _rep.GetTreeViewDataList(keyword).ToList();
        }
    }
}
