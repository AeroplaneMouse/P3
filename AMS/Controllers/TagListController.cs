using System;
using AMS.Models;
using System.Text;
using AMS.Services.Interfaces;
using System.Collections.Generic;
using AMS.Controllers.Interfaces;
using System.Collections.ObjectModel;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;

namespace AMS.Controllers
{
    public class TagListController : ITagListController
    {
        private readonly ITagRepository _rep;
        private IExporter _exporter;

        public ObservableCollection<Tag> TagsList { get; set; }
        public IUserListController UserListController { get; set; }

        public TagListController(ITagRepository rep, IExporter exporter)
        {
            _rep = rep;
            _exporter = exporter;

        }

        public void Search(string query)
        {
            TagsList = _rep.Search("");

        }

        public void AddNew()
        {
            //Todo redirect to tagEditor
            throw new NotImplementedException();
        }

        public void Edit(Tag tag)
        {
            //Todo redirect to tagEditor
            throw new NotImplementedException();
        }

        public void ViewTag(Tag tag)
        {
            //Todo redirect to tagViewer
            throw new NotImplementedException();
        }

        public void Remove(Tag tag)
        {
            if (TagsList.Contains(tag))
            {
                TagsList.Remove(tag);
            }
        }

        public void Export(List<Tag> tags)
        {
            throw new NotImplementedException();
        }
    }
}
