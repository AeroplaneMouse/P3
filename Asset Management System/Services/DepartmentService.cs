using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Services.Interfaces;

namespace Asset_Management_System.Services
{
    public class DepartmentService : IDepartmentService
    {
        private IDepartmentRepository _rep;

        public DepartmentService(IDepartmentRepository rep)
        {
            _rep = rep;
        }

        public IRepository<Department> GetRepository() => _rep;

        public string GetName(Department department) => department.Name;
    }
}