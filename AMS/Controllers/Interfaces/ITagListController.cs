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

        ITagService TagService { get; set; }

        void Search();

    }
}
