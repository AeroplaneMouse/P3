using System;
using AMS.Models;
using System.Text;
using AMS.Services.Interfaces;
using AMS.Controllers.Interfaces;
using System.Collections.Generic;
using AMS.Database.Repositories.Interfaces;
using System.Collections.ObjectModel;

namespace AMS.Controllers
{
    public class TagListController : ITagListController
    {
        private ITagRepository _rep;

        public ObservableCollection<Tag> TagsList { get; set; }
        public ITagService TagService { get; set; }

        public TagListController(ITagRepository rep)
        {
            _rep = rep;

        }

        public void Search()
        {
            TagsList = _rep.Search("");

        }
    }
}
