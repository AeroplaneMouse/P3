using Asset_Management_System.Models;

namespace Asset_Management_System.ViewModels.Interfaces
{
    public interface IFieldManager
    {
        bool AddField(DoesContainFields obj, Field field);
        bool UpdateField(DoesContainFields obj, Field field);
        bool RemoveField(DoesContainFields obj, Field field);
    }
}