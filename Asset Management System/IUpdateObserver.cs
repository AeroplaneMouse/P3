using Asset_Management_System.Models;

namespace Asset_Management_System
{
    public interface IUpdateObserver
    {
        void Update(Model Subject, bool delete);
    }
}