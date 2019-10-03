using System.Collections.Generic;
using Asset_Management_System.Models;

namespace Asset_Management_System.Database.Repositories
{
    interface IAssetRepository : IMysqlRepository<Asset>
    {
        List<Asset> SearchByTags(List<int> tags_ids);
        List<Asset> Search(string keyword);
    }
}