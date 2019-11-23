using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.Models;

namespace AMS.Controllers
{
    public class AssetListController : IAssetListController
    {
        private IExporter _exporter;
        private IAssetRepository _assetRepository;
        //TODO: Create tagging class
        //public Tagging _tags;

        public ObservableCollection<Asset> AssetList { get; set; }

        private List<List<ITagable>> _tagables;

        private Dictionary<ulong, List<ITagable>> _tagDict;

        public List<List<ITagable>> TagList
        {
            get
            {
                if (_tagables == null)
                {
                    for (int i = 0; i < AssetList.Count; i++)
                    {
                        _tagables[i].AddRange(GetTags(AssetList[i]));
                    }
                }

                return _tagables;
            }

            set => _tagables = value;
        }

        public Dictionary<ulong, List<ITagable>> TagDict
        {
            get
            {
                if (_tagDict == null)
                {
                    Dictionary<ulong, List<ITagable>> dict = new Dictionary<ulong, List<ITagable>>();

                    for (int i = 0; i < AssetList.Count; i++)
                    {
                        dict.Add(AssetList[i].ID, GetTags(AssetList[i]));
                    }

                    _tagDict = dict;
                }

                return _tagDict;
            }

            set => _tagDict = value;
        }

        public AssetListController(IAssetRepository assetRepository, IExporter exporter)
        {
            AssetList = new ObservableCollection<Asset>();
            _assetRepository = assetRepository;
            _exporter = exporter;
            Search("");
        }
        
        /// <summary>
        /// Filters the list if Assets to only contain assets that match the searchQuery
        /// </summary>
        /// <param name="query"></param>
        public void Search(string query, List<ulong> tags=null, List<ulong> users=null, bool strict=false)
        {
            //TODO: Filter list based on search query, also search by tags
            ObservableCollection<Asset> searchResult = _assetRepository.Search(query, tags, users, strict);
            AssetList = searchResult;
            //TODO: Determine if search should be on assets or by tags
            /* Remove comment when tagging class is implementet
            SearchList = _rep.Search(SearchQueryText,
                Tags.AppliedTags.OfType<Tag>().Select(t => t.ID).ToList(),
                Tags.AppliedTags.OfType<User>().Select(u => u.ID).ToList(),
                IsStrict);
                */
        }

        /// <summary>
        /// Removes the given asset from the database and from the list
        /// </summary>
        /// <param name="asset"></param>
        public void Remove(Asset asset)
        {
            // Check if asset is in list
            if (!AssetList.Contains(asset))
                return;
            // Delete the asset
            if (_assetRepository.Delete(asset))
                AssetList.Remove(asset);
        }

        /// <summary>
        /// Exports the selected items in the listView to a .csv file
        /// Using an IExporter
        /// </summary>
        /// <param name="assets"></param>
        public void Export(List<Asset> assets) => _exporter.Print(assets);

        public List<ITagable> GetTags(Asset asset) => _assetRepository.GetTags(asset).ToList();
    }
}