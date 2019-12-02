using System.Linq;
using AMS.Controllers.Interfaces;
using AMS.Database.Repositories;
using AMS.Database.Repositories.Interfaces;

namespace AMS.ViewModels
{
    public class HomeViewModel : Base.BaseViewModel
    {
        public ulong NumberOfUsers => _homeController.NumberOfUsers;
        public ulong NumberOfAssets => _homeController.NumberOfAssets;
        public ulong NumberOfTags => _homeController.NumberOfTags;
        public ulong NumberOfDepartments => _homeController.NumberOfDepartments;

        private IHomeController _homeController { get; set; }

        /// <summary>
        /// Default contructor
        /// </summary>
        public HomeViewModel(IHomeController controller)
        {
            _homeController = controller;

            // Notify view
            UpdateOnFocus();
        }

        public override void UpdateOnFocus()
        {
            OnPropertyChanged(nameof(NumberOfUsers));
            OnPropertyChanged(nameof(NumberOfAssets));
            OnPropertyChanged(nameof(NumberOfTags));
            OnPropertyChanged(nameof(NumberOfDepartments));
        }
    }
}