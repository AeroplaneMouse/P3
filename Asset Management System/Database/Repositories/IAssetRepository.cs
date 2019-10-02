using System.Collections.Generic;
using Asset_Management_System.Models;

namespace Asset_Management_System.Database.Repositories
{
    interface IAssetRepository : IRepository<Asset>
    {
        List<Asset> SearchByTags(List<int> tags_ids);
    }
}