using System;
using AMS.Models;
using System.Text;
using AMS.Services.Interfaces;
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

        public TagListController(PrintHelper printHelper)
        {
            _rep = new TagRepository();
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

        public List<Tag> GetParentTags()
        {
            return _rep.GetParentTags().ToList();
        }
        
        public List<Tag> GetChildTags(ulong id)
        {
            return _rep.GetChildTags(id).ToList();
        }
    }
}
