using System;
using System.Collections.Generic;
using System.Text;

namespace Asset_Management_System.ViewModels
{
    class HomeViewModel : Base.BaseViewModel
    {
        #region Constructor
        
        /// <summary>
        /// Default contructor
        /// </summary>
        public HomeViewModel(MainViewModel main)
        {
            // Initialize commands
            _main = main;
        }

        #endregion

        #region Private Properties

        private MainViewModel _main;

        #endregion

        #region Public Properties

        #endregion

        #region Commands

        #endregion

    }
}
