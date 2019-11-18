using System;
using AMS.Models;
using System.Text;
using AMS.Services.Interfaces;
using System.Collections.Generic;
using AMS.Controllers.Interfaces;
using System.Collections.ObjectModel;
using AMS.Database.Repositories.Interfaces;

namespace AMS.Controllers
{
    public class TagListController : ITagListController
    {
        private readonly ITagRepository _rep;

        public ObservableCollection<Tag> TagsList { get; set; }

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
