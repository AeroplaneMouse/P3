using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;

namespace Asset_Management_System.ViewModels.Interfaces
{
    public interface IObjectManager
    {
        bool Save();
        bool Remove();
        bool Update();
    }
}