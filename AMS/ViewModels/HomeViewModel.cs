using System.Linq;
using AMS.Database.Repositories;
using AMS.Database.Repositories.Interfaces;

namespace AMS.ViewModels
{
    public class HomeViewModel : Base.BaseViewModel
    {
        
        public ulong NumberOfUsers { get; set; }
        public ulong NumberOfAssets { get; set; }
        public ulong NumberOfTags { get; set; }
        public ulong NumberOfDepartments { get; set; }

        /// <summary>
        /// Default contructor
        /// </summary>
        public HomeViewModel(IUserRepository userRepository, IAssetRepository assetRepository, ITagRepository tagRepository, IDepartmentRepository departmentRepository)
        {
            // Get the number of stored assets, tags and departments
            NumberOfUsers = (ulong)userRepository.GetAll().Count(p => p.IsEnabled);
            //TODO: fix so it uses Dependency Injection
            NumberOfAssets = assetRepository.GetCount();
            NumberOfTags = tagRepository.GetCount();
            NumberOfDepartments = departmentRepository.GetCount();

            // Notify view
            OnPropertyChanged(nameof(NumberOfUsers));
            OnPropertyChanged(nameof(NumberOfAssets));
            OnPropertyChanged(nameof(NumberOfTags));
            OnPropertyChanged(nameof(NumberOfDepartments));
        }
    }
}