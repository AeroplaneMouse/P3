using AMS.Database.Repositories;
using AMS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMS.Models
{
    public class AssetWithTags : Asset
    {
        public List<ITagable> Tags { get; set; }

        public Asset Asset { get; set; }

        public AssetWithTags(Asset asset)
        {
            Asset = asset;
        }
    }
}
