using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Asset_Management_System.Database.Repositories
{
    public interface ISearchableRepository<T>
    {
        ObservableCollection<T> Search(string keyword);
    }
}
