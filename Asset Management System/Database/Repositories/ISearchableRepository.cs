using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Asset_Management_System.Database.Repositories
{
    public interface ISearchableRepository<T>
    {
        ObservableCollection<T> Search(string keyword, List<ulong> tags=null);
    }
}
