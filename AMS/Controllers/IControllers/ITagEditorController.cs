using AMS.Models;
using AMS.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Controllers.IControllers
{
    public interface ITagEditorController
    {
        #region Properties

        Tag tag { get; set; }

        ITagService TagService { get; set; }

        #endregion

        #region Methods

        void Save();

        #endregion
    }
}
