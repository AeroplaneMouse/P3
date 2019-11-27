using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AMS.Database.Repositories.Interfaces
{
    public interface ISearchableRepository<T>
    {
        List<T> Search(string keyword, List<ulong> tags=null, List<ulong> users=null, bool strict=false);
    }
}
