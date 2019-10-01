using System;
using System.Collections.Generic;
using System.Text;
using Asset_Management_System.Models;

namespace Asset_Management_System.Database.Repositories
{
    interface IDepartmentRepository : IRepository<Department>
    {
        public List<Department> GetAll();
    }
}
