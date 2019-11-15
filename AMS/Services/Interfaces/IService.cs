using System.Windows.Controls;
using AMS.Models;
using AMS.Database.Repositories.Interfaces;

namespace AMS.Services.Interfaces
{
    public interface IService<T>
    {
        IRepository<T> GetRepository();

        string GetName(T obj);
    }
}