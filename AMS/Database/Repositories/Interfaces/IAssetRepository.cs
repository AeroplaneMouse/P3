using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AMS.Interfaces;
using AMS.Models;

namespace AMS.Database.Repositories.Interfaces
{
    public interface IAssetRepository : IMysqlRepository<Asset>, ISearchableRepository<Asset>
    {
        ulong GetCount();

        bool AttachTags(Asset asset, List<ITagable> tagged);
        IEnumerable<ITagable> GetTags(Asset asset);
    }
}