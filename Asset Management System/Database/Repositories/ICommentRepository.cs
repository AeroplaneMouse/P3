using System.Collections.ObjectModel;
using Asset_Management_System.Models;

namespace Asset_Management_System.Database.Repositories
{
    public interface ICommentRepository : IMysqlRepository<Comment>
    {
        ObservableCollection<Comment> GetByAssetId(ulong assetId);
    }
}
