using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Controllers.Interfaces
{
    public interface IHomeController 
    {
        ulong NumberOfUsers { get; set; }
        ulong NumberOfAssets { get; set; }
        ulong NumberOfTags { get; set; }
        ulong NumberOfDepartments { get; set; }
    }
}
