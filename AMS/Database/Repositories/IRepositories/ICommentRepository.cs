using System.Collections.ObjectModel;
using AMS.Models;

namespace AMS.Database.Repositories
{
    public interface ICommentRepository : IMysqlRepository<Comment>
    {
        ObservableCollection<Comment> GetByAssetId(ulong assetId);
    }
}
