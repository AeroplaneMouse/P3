using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Models;
using AMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Controllers
{
    public class TagController : ITagController
    {
        public Tag tag { get; set; }
        public ITagService TagService { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Save()
        {
            ITagRepository repository = (ITagRepository)TagService.GetRepository();
        }
    }
}
