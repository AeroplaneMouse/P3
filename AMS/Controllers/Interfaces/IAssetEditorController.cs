using AMS.Models;
using AMS.Services;
using AMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Controllers.Interfaces
{
    public interface IAssetEditorController
    {
        #region Properties

        Asset Asset { get; set; }

        IAssetService AssetService { get; set; }

        ITagService TagService { get; set; }

        #endregion

        #region Methods

        void Save();

        void AttachTag(List<ITagable> tagables);

        #endregion
    }
}
