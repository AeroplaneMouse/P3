using System.Collections.Generic;
using Asset_Management_System.Models;

namespace Asset_Management_System.Database.Repositories
{
    public interface ITagRepository : IMysqlRepository<Tag>, ISearchableRepository<Tag>
    {
        IEnumerable<Tag> GetAll();

        IEnumerable<Tag> GetTagsForAsset(ulong id);
        IEnumerable<Tag> GetParentTags();
        IEnumerable<Tag> GetChildTags(ulong parentId);
        ulong GetCount();
    }
}
