using Asset_Management_System.Authentication;
using Asset_Management_System.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels
{
    public class SplashViewModel : Base.BaseViewModel
    {
        public event StatusUpdateEventHandler StatusUpdate;

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

        public string LoadingText { get; set; } = "Loading...";
        public string StatusText { get; set; }

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
            Session t = new Session();
            
            if (t.Authenticated())
                _main.SystemLoaded();
            else
                UpdateStatusText(new StatusUpdateEventArgs("User not authorized to access the application."));
        }

        public void UpdateStatusText(StatusUpdateEventArgs e)
        {
            StatusText = e.Message;
        }

        #endregion
    }
}
