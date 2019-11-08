using System.Windows.Controls;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.ViewModels;

namespace Asset_Management_System.Services.Interfaces
{
    public interface IService<T>
    {
        ISearchableRepository<T> GetSearchableRepository();
        
        IRepository<T> GetRepository();

        Page GetManagerPage(MainViewModel main, T inputAsset = default, bool addMultiple = false);

        string GetName(T obj);

    }
}