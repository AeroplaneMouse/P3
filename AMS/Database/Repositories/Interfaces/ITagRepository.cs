using System.Collections.Generic;
using AMS.Models;

namespace AMS.Database.Repositories.Interfaces
{
    public interface ITagRepository : IMySqlRepository<Tag>, ISearchableRepository<Tag>
    {
        IEnumerable<Tag> GetAll();
        IEnumerable<Tag> GetTagsForAsset(ulong id);
        IEnumerable<Tag> GetParentTags();
        IEnumerable<Tag> GetChildTags(ulong parentID);
        IEnumerable<Tag> GetTreeViewDataList(string keyword = "");
        ulong GetCount();
        bool Delete(Tag entity, bool removeChildren);
    }
}
