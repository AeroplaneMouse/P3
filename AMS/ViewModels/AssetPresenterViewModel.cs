using AMS.Controllers.Interfaces;
using AMS.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AMS.ViewModels
{
    class AssetPresenterViewModel : Base.BaseViewModel
    {
        private IAssetController _assetController { get; set; }

        public ObservableCollection<object> Tabs { get; }
        public int SelectedTabIndex { get; set; }
        public string Name => _assetController.ControlledAsset.Name;
        public string ID => $" (ID: { _assetController.ControlledAsset.ID })";
        public ICommand RemoveCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand CancelCommand { get; set; }
   
        public AssetPresenterViewModel(IAssetController assetController, ICommentListController commentListController, ILogListController logListController, int tabIndex = 0)
        {
            _assetController = assetController;

            // Tabs
            Tabs = new ObservableCollection<object>();
            Tabs.Add(new AssetDetailsViewModel(_assetController));
            Tabs.Add(new CommentViewModel(commentListController));
            Tabs.Add(new LogListViewModel(logListController));

            SelectedTabIndex = tabIndex;

            EditCommand = new Base.RelayCommand(() => Edit(), () => Features.GetCurrentSession().IsAdmin());
            RemoveCommand = new Base.RelayCommand(() => Remove(), () => Features.GetCurrentSession().IsAdmin());
            CancelCommand = new Base.RelayCommand(Cancel);
        }

        public override void UpdateOnFocus()
        {
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(ID));

            (Tabs[0] as AssetDetailsViewModel).UpdateOnFocus();
            (Tabs[1] as CommentViewModel).UpdateOnFocus();
            (Tabs[2] as LogListViewModel).UpdateOnFocus();

            OnPropertyChanged(nameof(Tabs));
        }

        /// <summary>
        /// Removes the viewed asset
        /// </summary>
        private void Remove()
        {
            Features.DisplayPrompt(new Views.Prompts.Confirm($"Are you sure you want to remove { _assetController.ControlledAsset.Name }?", (sender, e) =>
            {
                if (e.Result)
                {
                    _assetController.Remove();
                    Features.AddNotification(new Notification($"{ _assetController.ControlledAsset.Name } has been deleted", Notification.INFO));
                    Features.Navigate.Back();
                }
            }));
        }

        /// <summary>
        /// Edits the viewed asset
        /// </summary>
        private void Edit ()
        {
            Features.Navigate.To(Features.Create.AssetEditor(_assetController));
        }

        /// <summary>
        /// Goes back to the previous page
        /// </summary>
        private void Cancel()
        {
            if (!Features.Navigate.Back())
                Features.Navigate.To(Features.Create.AssetList());
        }
    }
}
