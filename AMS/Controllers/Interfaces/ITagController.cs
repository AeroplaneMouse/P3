using AMS.Database.Repositories.Interfaces;
using AMS.Models;
using AMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Controllers.Interfaces
{
    public interface ITagController
    {
        #region Properties

        Tag tag { get; set; }

        ITagRepository tagRepository { get; set; }

        #endregion

        #region Methods

        void Save();

        void Remove();

        void Update();

        #endregion
    }
}
