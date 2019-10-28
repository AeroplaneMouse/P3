using System;
using System.Collections.Generic;
using System.Text;

namespace Asset_Management_System.ViewModels
{
    public class CommentsViewModel : Base.BaseViewModel
    {
        #region Private Properties

        private MainViewModel _main;

        #endregion

        #region Public Properties

        #endregion

        public CommentsViewModel(MainViewModel main)
        {
            _main = main;
        }
    }
}
