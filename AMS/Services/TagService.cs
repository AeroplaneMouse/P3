using AMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using AMS.Database.Repositories.Interfaces;
using AMS.Models;

namespace AMS.Services
{
    public class TagService : ITagService
    {
        public IRepository<Tag> GetRepository()
        {
            throw new NotImplementedException();
        }

        public string GetName(Tag obj)
        {
            throw new NotImplementedException();
        }
    }
}
