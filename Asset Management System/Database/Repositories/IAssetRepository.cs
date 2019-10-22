using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Asset_Management_System.Models;

namespace Asset_Management_System.Database.Repositories
{
    interface IAssetRepository : IMysqlRepository<Asset>, ISearchableRepository<Asset>
    {
        IEnumerable<Asset> SearchByTags(List<int> tags_ids);
        bool AttachTagsToAsset(Asset asset, List<Tag> tags);
        List<Tag> GetAssetTags(Asset asset);
        Int32 GetCount();
    }
}