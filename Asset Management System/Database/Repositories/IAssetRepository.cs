using System.Collections.Generic;
using System.Collections.ObjectModel;
using Asset_Management_System.Models;

namespace Asset_Management_System.Database.Repositories
{
    interface IAssetRepository : IMysqlRepository<Asset>
    {
        IEnumerable<Asset> SearchByTags(List<int> tags_ids);
        ObservableCollection<Asset> Search(string keyword);
        bool AttachTagsToAsset(Asset asset, List<Tag> tags);
        List<Tag> GetAssetTags(Asset asset);
    }
}