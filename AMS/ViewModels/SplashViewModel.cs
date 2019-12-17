using System;
using AMS.Database;
using System.Threading;
using AMS.Authentication;
using System.Windows.Input;
using System.Threading.Tasks;
using AMS.Database.Repositories;
using AMS.Database.Repositories.Interfaces;

namespace AMS.ViewModels
{
    class SplashViewModel : Base.BaseViewModel
    {
        private MainViewModel _main;
        private IUserRepository _userRepository;
        private const int _delay = 0;
        private const int _reconnectWaitingTime = 5;
        private bool _configuring = false;

        public string LoadingText { get; set; }
        public string CurrentActionText { get; set; }
        public string AdditionalText { get; set; }
        public ICommand LoadConfigCommand { get; set; }


        public SplashViewModel(MainViewModel main, IUserRepository userRepository)
        {
            _main = main;
            _userRepository = userRepository;

            // Initializing commands
            LoadConfigCommand = new Base.RelayCommand(LoadConfig);

            Setup();
        }

        public override void UpdateOnFocus() { }

        /// <summary>
        /// Establishing a connection to the database and authorizing the user. This function runs asynchronous
        /// meaning that it does not halt the UI-thread while doing this.
        /// </summary>
        private async void Setup()
        {
            // Check for settings file
            if (String.IsNullOrEmpty(Session.GetDBKey()))
            {
                LoadingText = "No configuration";
                CurrentActionText = "Create a configuration through the button below";
                return;
            }

            // Try to connect to database from the set settings and authenticate user.
            bool reconnectRequired;
            do
            {
                reconnectRequired = await Task.Run(Authenticate);
            } while (reconnectRequired && !_configuring);
        }

        /// <summary>
        /// Checks if the logged in user has access to the system
        /// </summary>
        /// <returns></returns>
        private bool Authenticate()
        {
            // Connecting to database
            LoadingText = "Establishing connection...";
            CurrentActionText = "A connection to the database is being established...";
            AdditionalText = "";

            Thread.Sleep(_delay);

            if (new MySqlHandler().IsAvailable())
            {
                // Authorizing user
                LoadingText = "Connection established...";
                CurrentActionText = "The connection to the database was succesfully established...";
                Thread.Sleep(_delay);

                Session t = new Session(_userRepository);
                if (t.Authenticated())
                {
                    // Runs the systemLoaded method to remove splashpage, and 
                    LoadingText = "User authenticated";
                    CurrentActionText = "Unlocking the Asset Management System...";
                    Thread.Sleep(_delay);

                    _main.LoadSystem(t);
                }
                else
                {
                    LoadingText = "!!! Access denied !!!";
                    CurrentActionText = $"User \"{Session.GetIdentity()}\" is not authorized to access the application.";
                }

                // Reconnect is not required
                return false;
            }
            else
            {
                LoadingText = "ERROR!";
                CurrentActionText = "Unable to connect to the database.";
                Reconnect();

                // Reconnect is required
                return true;
            }
        }

        /// <summary>
        /// A delay before reconnecting
        /// </summary>
        private void Reconnect()
        {
            const string baseText = "Reconnecting in";
            for (int i = _reconnectWaitingTime; i > 0; i--)
            {
                AdditionalText = $"{ baseText } { i }...";
                Thread.Sleep(1000);
            }
            AdditionalText = "";
        }

        /// <summary>
        /// Prompts the user to input a config file
        /// </summary>
        private void LoadConfig()
        {
            // Stop reconnecting
            _configuring = true;
            
            // Move to the settings editor
            _main.SplashPage = Features.Create.SettingsEditor(this);
            // Manually calls the update function as the page is not navigated to using the navigator.
            (_main.SplashPage.DataContext as SettingsEditorViewModel).UpdateOnFocus();
        }
    }
}
