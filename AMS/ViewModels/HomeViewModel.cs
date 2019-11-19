using System.Linq;
using AMS.Database.Repositories;

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
        public HomeViewModel()
        {
            // Get the number of stored assets, tags and departments
            NumberOfUsers = (ulong) new UserRepository().GetAll().Count(p => p.IsEnabled);
            //TODO: fix so it uses Dependency Injection
            NumberOfAssets = new AssetRepository().GetCount();
            NumberOfTags = new TagRepository().GetCount();
            NumberOfDepartments = new DepartmentRepository().GetCount();

            // Notify view
            OnPropertyChanged(nameof(NumberOfUsers));
            OnPropertyChanged(nameof(NumberOfAssets));
            OnPropertyChanged(nameof(NumberOfTags));
            OnPropertyChanged(nameof(NumberOfDepartments));
        }
    }
}