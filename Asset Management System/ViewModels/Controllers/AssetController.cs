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

        public AssetController(Asset inputAsset, IAssetService service, bool addMultiple = false)
        {
            _service = service;
            _assetRep = (IAssetRepository) _service.GetSearchableRepository();
            
            FieldsList = new ObservableCollection<ShownField>();
            if (inputAsset != null)
            {
                Asset = inputAsset;
                LoadFields();

                CurrentlyAddedTags = new ObservableCollection<Tag>(_assetRep.GetAssetTags(Asset));

                ConnectTags();
                if (!addMultiple)
                    Editing = true;
            }
            else
            {
                CurrentlyAddedTags = new ObservableCollection<Tag>();
                Asset = new Asset();
                Editing = false;
            }
        }
        
        /// <summary>
        /// This function loads the fields from the asset, and into the viewmodel.
        /// </summary>
        protected override void LoadFields()
        {
            foreach (var field in Asset.FieldsList)
            {
                if (field.IsHidden)
                    HiddenFields.Add(new ShownField(field));
                else
                    FieldsList.Add(new ShownField(field));
            }
        }
    }
}