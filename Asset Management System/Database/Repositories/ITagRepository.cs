using System;
using System.Collections.Generic;
using System.Text;
using Asset_Management_System.Models;

namespace Asset_Management_System.Database.Repositories
{
    interface ITagRepository : IRepository<Tag>
    {
        public Department GetDepartment();
        public List<Tag> GetChildTags(long parent_id);
        public Tag GetParentTag();
    }
}
