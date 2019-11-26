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
        Asset _asset { get; set; }
        public string Name { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommentListController CommentListController { get; set; }

        ObservableCollection<object> _children;

        public AssetPresenterViewModel(Asset asset, List<ITagable> tagList, ICommentListController commentListController, ILogListController logListController)
        {
            _asset = asset;
            Name = asset.Name;
            CommentListController = commentListController;

            _children = new ObservableCollection<object>();
            _children.Add(new AssetDetailsViewModel(asset, tagList));
            _children.Add(new CommentViewModel(asset, commentListController));
            _children.Add(new LogListViewModel(logListController));

            EditCommand = new Base.RelayCommand(Edit);
            CancelCommand = new Base.RelayCommand(Cancel);
        }

        private void Edit ()
        {
            Features.Navigate.To(Features.Create.AssetEditor(_asset));
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
