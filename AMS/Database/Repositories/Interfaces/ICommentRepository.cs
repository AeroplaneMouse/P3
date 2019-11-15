using System.Collections.ObjectModel;
using AMS.Models;

namespace AMS.Database.Repositories.Interfaces
{
    public interface ICommentRepository : IMysqlRepository<Comment>
    {
        ObservableCollection<Comment> GetByAssetId(ulong assetId);
    }
}
