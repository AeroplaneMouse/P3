using System.Collections.Generic;
using AMS.Models;

namespace AMS.Database.Repositories.Interfaces
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
