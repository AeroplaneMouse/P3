﻿using System.Collections.Generic;
using Asset_Management_System.Models;

namespace Asset_Management_System.Database.Repositories
{
    interface ITagRepository : IMysqlRepository<Tag>, ISearchableRepository<Tag>
    {
        IEnumerable<Tag> GetAll();
        IEnumerable<Tag> GetParentTags();
        IEnumerable<Tag> GetChildTags(ulong parentId);
        ulong GetCount();
    }
}
