using Asset_Management_System.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels
{
    public class SplashViewModel : Base.BaseViewModel
    {
        #region Constructors

        public SplashViewModel(MainViewModel main)
        {
            _main = main;

            // Initializing commands
            LoadConfigCommand = new Base.RelayCommand(() => LoadConfig());
            Console.WriteLine("Showing splash screen");
            Authenticate();
        }

        #endregion

        #region Private Properties

        private MainViewModel _main;

        #endregion

        #region Public Properties

        #endregion

        #region Commands

        public ICommand LoadConfigCommand { get; set; }

        #endregion

        #region Methods

        private void LoadConfig()
        {
            throw new NotImplementedException();
        }

        private void Authenticate()
        {
            if (new Session().Validate())
                _main.SystemLoaded();
        }

        #endregion
    }
}
