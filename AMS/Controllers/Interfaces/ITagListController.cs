using AMS.Database.Repositories.Interfaces;
using AMS.Models;
using AMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AMS.Controllers.Interfaces
{
    public interface ITagListController
    {
        ObservableCollection<Tag> TagsList { get; set; }

        void Search();

    }
}
