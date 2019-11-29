using AMS.Controllers.Interfaces;
using AMS.Helpers;
using AMS.Interfaces;
using AMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace AMS.ViewModels
{
    class AssetPresenterViewModel : Base.BaseViewModel
    {
        private IAssetController _assetController { get; set; }

        private ObservableCollection<object> _tabs;


        public ObservableCollection<object> Tabs => _tabs;
        public string Name { get; set; }
        public string ID { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        


        public AssetPresenterViewModel(List<ITagable> tagList, IAssetController assetController, ICommentListController commentListController, ILogListController logListController)
        {
            Name = assetController.ControlledAsset.Name;
            ID = $" (id: { assetController.ControlledAsset.ID })";
            _assetController = assetController;

            // Tabs
            _tabs = new ObservableCollection<object>();
            _tabs.Add(new AssetDetailsViewModel(assetController.ControlledAsset, tagList));
            _tabs.Add(new CommentViewModel(assetController.ControlledAsset, commentListController));
            _tabs.Add(new LogListViewModel(logListController));

            EditCommand = new Base.RelayCommand(() => Edit(), () => Features.GetCurrentSession().IsAdmin());
            RemoveCommand = new Base.RelayCommand(() => Remove(), () => Features.GetCurrentSession().IsAdmin());
            CancelCommand = new Base.RelayCommand(Cancel);
        }

        public override void UpdateOnFocus()
        {
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(ID));
            OnPropertyChanged(nameof(Tabs));
        }

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

        private void Edit ()
        {
            Features.Navigate.To(Features.Create.AssetEditor(_assetController.ControlledAsset));
        }

        private void Cancel()
        {
            if (Features.Navigate.Back() == false)
            {
                Features.Navigate.To(Features.Create.AssetList());
            }
        }
        

    }
}
