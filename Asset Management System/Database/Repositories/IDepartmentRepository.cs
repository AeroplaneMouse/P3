﻿using System.Collections.Generic;
using Asset_Management_System.Models;

namespace Asset_Management_System.Database.Repositories
{
    public interface IDepartmentRepository : IMysqlRepository<Department>
    {
        IEnumerable<Department> GetAll();
        ulong GetCount();
    }
}
