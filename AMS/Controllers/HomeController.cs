using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMS.Controllers
{
    public class HomeController : IHomeController
    {
        public ulong NumberOfUsers => (ulong)_userRepository.GetAll().Count(p => p.IsEnabled);
        public ulong NumberOfAssets => _assetRepository.GetCount();
        public ulong NumberOfTags => _tagRepository.GetCount();
        public ulong NumberOfDepartments => _departmentRepository.GetCount();

        private IUserRepository _userRepository { get; set; }
        private IAssetRepository _assetRepository { get; set; }
        private ITagRepository _tagRepository { get; set; }
        private IDepartmentRepository _departmentRepository { get; set; }


        public HomeController(IUserRepository userRepository, IAssetRepository assetRepository, ITagRepository tagRepository, IDepartmentRepository departmentRepository)
        {
            _userRepository = userRepository;
            _assetRepository = assetRepository;
            _tagRepository = tagRepository;
            _departmentRepository = departmentRepository;
        }

        /// <summary>
        /// Gets the tags currently attached to the input asset
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public List<ITagable> GetTags(Asset asset)
        {
            return _assetRepository.GetTags(asset).ToList();
        }

        /// <summary>
        /// Gets an asset by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Asset GetAsset(ulong id)
        {
            return _assetRepository.GetById(id);
        }

    }
}
