using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Asset_Management_System.Models;

namespace Asset_Management_System.Database.Repositories
{
    public interface IAssetRepository : IMysqlRepository<Asset>, ISearchableRepository<Asset>
    {
        ObservableCollection<Asset> SearchByTags(List<int> tagsIds);
        bool AttachTagsToAsset(Asset asset, List<Tag> tags);
        List<Tag> GetAssetTags(Asset asset);
        ulong GetCount();
    }
}