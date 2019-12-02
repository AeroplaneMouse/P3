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
        public ulong NumberOfUsers
        {
            get => (ulong)_userRepository.GetAll().Count(p => p.IsEnabled);
            set => NumberOfUsers = value;
        }
        public ulong NumberOfAssets
        {
            get => _assetRepository.GetCount();
            set => NumberOfAssets = value;
        }
        public ulong NumberOfTags
        {
            get => _tagRepository.GetCount();
            set => NumberOfTags = value;
        }
        public ulong NumberOfDepartments
        {
            get => _departmentRepository.GetCount();
            set => NumberOfDepartments = value;
        }

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

        public List<ITagable> GetTags(Asset asset)
        {
            return _assetRepository.GetTags(asset).ToList();
        }

        public Asset GetAsset(ulong id)
        {
            return _assetRepository.GetById(id);
        }

    }
}
