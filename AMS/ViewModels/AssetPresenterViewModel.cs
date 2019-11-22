using AMS.Controllers.Interfaces;
using AMS.Interfaces;
using AMS.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.ViewModels
{
    class AssetPresenterViewModel : Base.BaseViewModel
    {
        public string PageTitle { get; set; }
        public AssetPresenterViewModel(Asset asset, List<ITagable> tagList, ICommentListController commentListController)
        {
            PageTitle = asset.Name;
        }
    }
}
