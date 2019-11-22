using AMS.Controllers.Interfaces;
using AMS.Interfaces;
using AMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace AMS.ViewModels
{
    class AssetPresenterViewModel : Base.BaseViewModel
    {
        private Asset _Asset { get; set; }
        public string Name {
            get {
                return _Asset.Name;
            } }
        public string Identifier {
            get {
                return _Asset.Identifier;
            }
        }
        public string Description {
            get {
                return _Asset.Description;
            }
        }
        public List<ITagable> TagList { get; set; }

        public ObservableCollection<Field> FieldList {
            get {
                return new ObservableCollection<Field>(_Asset.FieldList);
            }
        }

        public ICommentListController CommentListController { get; set; }

        ICommand EditCommand { get; set; }
        ICommand CancelCommand { get; set; }

        public AssetPresenterViewModel(Asset asset, List<ITagable> tagList, ICommentListController commentListController)
        {
            TagList = tagList;
            _Asset = asset;
            CommentListController = commentListController;

            //EditCommand = new Base.RelayCommand(Edit);
            //CancelCommand = new Base.RelayCommand(Cancel);
        }

        private void Edit (object asset)
        {

        }
    }
}
