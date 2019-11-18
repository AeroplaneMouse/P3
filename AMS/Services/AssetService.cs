using AMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using AMS.Database.Repositories.Interfaces;
using AMS.Models;

namespace AMS.Services
{
    public class AssetService : IAssetService
    {
        public IRepository<Asset> GetRepository()
        {
            throw new NotImplementedException();
        }

        public string GetName(Asset obj)
        {
            throw new NotImplementedException();
        }
    }
}
