using System.Collections.Generic;
using AMS.Models;

namespace AMS.Database.Repositories.Interfaces
{
    public interface IDepartmentRepository : IMysqlRepository<Department>
    {
        IEnumerable<Department> GetAll();
        ulong GetCount();
    }
}
