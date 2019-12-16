using AMS.Interfaces;
using AMS.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Controllers.Interfaces
{
    public interface IHomeController 
    {
        ulong NumberOfUsers { get; }
        ulong NumberOfAssets { get; }
        ulong NumberOfTags { get; }
        ulong NumberOfDepartments { get; }

        List<ITagable> GetTags(Asset asset);
        Asset GetAsset(ulong id);
    }
}
