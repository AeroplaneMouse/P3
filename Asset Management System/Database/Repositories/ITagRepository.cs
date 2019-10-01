using System.Collections.Generic;
using Asset_Management_System.Models;

namespace Asset_Management_System.Database.Repositories
{
    interface ITagRepository : IRepository<Tag>
    {
        Department GetDepartment();
        List<Tag> GetChildTags(long parent_id);
        Tag GetParentTag();
    }
}
