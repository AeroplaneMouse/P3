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
        private IAssetController _assetController;

        public string Name { get; set; }
        public string ID { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommentListController CommentListController { get; set; }

        ObservableCollection<object> _children;

        public AssetPresenterViewModel(List<ITagable> tagList, IAssetController assetController, ICommentListController commentListController, ILogListController logListController)
        {
            Name = assetController.Asset.Name;
            ID = $" (id: { assetController.Asset.ID })";
            CommentListController = commentListController;
            _assetController = assetController;

            _children = new ObservableCollection<object>();
            _children.Add(new AssetDetailsViewModel(assetController.Asset, tagList));
            _children.Add(new CommentViewModel(assetController.Asset, commentListController));
            _children.Add(new LogListViewModel(logListController));

            EditCommand = new Base.RelayCommand(Edit);
            RemoveCommand = new Base.RelayCommand(Remove);
            CancelCommand = new Base.RelayCommand(Cancel);
        }

        private void Remove()
        {
            Features.DisplayPrompt(new Views.Prompts.Confirm($"Are you sure you want to remove { _assetController.Asset.Name }?", (sender, e) =>
            {
                if (e.Result)
                {
                    _assetController.Remove();
                    Features.AddNotification(new Notification($"{ _assetController.Asset.Name } has been deleted", Notification.INFO));
                    Features.Navigate.Back();
                }
            }));
        }

        private void Edit ()
        {
            Features.Navigate.To(Features.Create.AssetEditor(_assetController.Asset));
        }

        private void Cancel()
        {
            if (Features.Navigate.Back() == false)
            {
                Features.Navigate.To(Features.Create.AssetList());
            }
        }

        public ObservableCollection<object> Children { get { return _children; } }
    }
}
