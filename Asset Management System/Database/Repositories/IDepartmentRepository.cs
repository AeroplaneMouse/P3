using System.Collections.Generic;
using Asset_Management_System.Models;

namespace Asset_Management_System.Database.Repositories
{
    interface IDepartmentRepository : IRepository<Department>
    {
        List<Department> GetAll();
    }
}
