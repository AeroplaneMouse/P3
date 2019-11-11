using System.Windows.Controls;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.ViewModels;

namespace Asset_Management_System.Services.Interfaces
{
    public interface IDisplayableService<T> : IService<T>
    {
        ISearchableRepository<T> GetSearchableRepository();
        
        Page GetManagerPage(MainViewModel main, T inputAsset = default, bool addMultiple = false);
    }
}