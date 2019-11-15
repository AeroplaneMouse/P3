using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AMS.Models;

namespace AMS.Database.Repositories.Interfaces
{
    public interface IAssetRepository : IMysqlRepository<Asset>, ISearchableRepository<Asset>
    {
        ObservableCollection<Asset> SearchByTags(List<int> tagsIds);
        //bool AttachTagsToAsset(Asset asset, List<Tag> tags);
        //List<Tag> GetAssetTags(Asset asset);
        ulong GetCount();

        bool AttachTags(Asset asset, List<ITagable> tagged);
        IEnumerable<ITagable> GetTags(Asset asset);
    }
}