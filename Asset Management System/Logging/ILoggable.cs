using System.Collections.Generic;
using Asset_Management_System.Database.Repositories;

namespace Asset_Management_System.Logging
{
    public interface ILoggable<T>
    {
        Dictionary<string, string> GetLoggableProperties();

        string GetLoggableName();

        ulong GetId();

        IRepository<T> GetRepository();
    }
}