using AMS.Models;
using AMS.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Controllers.IControllers
{
    public interface ITagListController
    {
        #region Properties

        List<Tag> TagsList { get; set; }

        ITagService TagService { get; set; }

        #endregion

        #region Methods

        void Search();

        #endregion
    }
}
