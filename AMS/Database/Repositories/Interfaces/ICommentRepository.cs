using System.Collections.Generic;
using System.Collections.ObjectModel;
using AMS.Models;

namespace AMS.Database.Repositories.Interfaces
{
    public interface ICommentRepository : IMysqlRepository<Comment>
    {
        List<Comment> GetByAssetId(ulong assetId);

        List<Comment> GetAll(bool includeDeleted = false, int limit=100);

        List<Comment> GetLatestComments(ulong departmentId = 0, int limit = 100, int days = 7);
    }
}
