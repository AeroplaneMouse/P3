using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AMS.Database.Repositories.Interfaces
{
    public interface ISearchableRepository<T>
    {
        ObservableCollection<T> Search(string keyword, List<ulong> tags=null, List<ulong> users=null, bool strict=false);
    }
}
