using AMS.Controllers.Interfaces;
using AMS.Interfaces;
using AMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AMS.ViewModels
{
    class AssetPresenterViewModel : Base.BaseViewModel
    {
        public string Name { get; set; }
        public string Identifier { get; set; }
        public string Description { get; set; }
        public List<ITagable> TagList { get; set; }
        public ObservableCollection<Field> FieldList { get; set; }

        public ICommentListController CommentListController { get; set; }

        public AssetPresenterViewModel(Asset asset, List<ITagable> tagList, ICommentListController commentListController)
        {
            Name = asset.Name;
            Identifier = asset.Identifier;
            Description = asset.Description;
            FieldList = asset.FieldList;

            TagList = tagList;

            CommentListController = commentListController;
        }
    }
}
