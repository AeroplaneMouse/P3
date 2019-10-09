using System.Collections.Generic;
using Asset_Management_System.Models;

namespace Asset_Management_System.Database.Repositories
{
    interface ITagRepository : IMysqlRepository<Tag>
    {
        List<Tag> GetParentTags();
        List<Tag> GetChildTags(long parent_id);
        List<Tag> Search(string keyword);
    }
}
