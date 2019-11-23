using AMS.Database.Repositories.Interfaces;
using AMS.Models;
using AMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Controllers.Interfaces
{
    public interface ITagController : IFieldListController
    {
        #region Properties

        Tag Tag { get; set; }

        bool IsEditing { get; set; }

        List<Tag> ParentTagList { get; }

        #endregion

        #region Methods

        void Save();

        void Remove();

        void Update();

        string CreateRandomColor();

        #endregion
    }
}
