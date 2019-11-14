using System.Collections.ObjectModel;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Services.Interfaces;
using Asset_Management_System.ViewModels.ViewModelHelper;

namespace Asset_Management_System.ViewModels.Controllers
{
    public class AssetController : ObjectManagerController
    {
        private IAssetRepository _assetRep;

        private IAssetService _service;

        public Asset Asset { get; set; }

        public AssetController(Asset inputAsset, IAssetService service)
        {
            _service = service;
            _assetRep = (IAssetRepository) _service.GetSearchableRepository();

            if (inputAsset != null)
            {
                Asset = inputAsset;
                //Todo få implementeret funktioner i denne.

                //CurrentlyAddedTags = new ObservableCollection<ITagable>(_assetRep.GetTags(Asset));

                //ConnectTags();
            }
            else
            {
                //CurrentlyAddedTags = new ObservableCollection<ITagable>();
                //Asset = new Asset();
            }
        }
    }
}