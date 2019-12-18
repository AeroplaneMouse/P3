using AMS.Models;
using AMS.ViewModels;

namespace AMS.Helpers
{
    public class DepartmentHelper
    {
        public Department GetCurrentDepartment()
        {
            return Features.GetCurrentDepartment();
        }
    }
}