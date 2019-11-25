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
        private const int _delay = 300;
        private const int _reconnectWaitingTime = 5;

        public string LoadingText { get; set; }
        public string CurrentActionText { get; set; }
        public string AdditionalText { get; set; }
        public ICommand LoadConfigCommand { get; set; }


        public SplashViewModel(MainViewModel main, IUserRepository userRepository)
        {
            Console.WriteLine("Showing splash screen");
            _main = main;
            _userRepository = userRepository;

            // Initializing commands
            LoadConfigCommand = new Base.RelayCommand(() => LoadConfig());

            Setup();
        }

        /// <summary>
        /// Establishing a connection to the database and authorizing the user. This function runs asynchronous
        /// meaning that it does not halt the UI-thread while doing this.
        /// </summary>
        private async void Setup()
        {
            //UpdateStatusText(new StatusUpdateEventArgs("Loading...", "Initializing background worker..."));
            bool reconnectRequired;
            do
            {
                reconnectRequired = await Task.Run(Authenticate);
            } while (reconnectRequired);
        }

        private bool Authenticate()
        {
            // Connecting to database
            LoadingText = "Establishing connection...";

            // TODO: Er den her nødvendig?
            CurrentActionText = "A connection to the database is being established...";
            AdditionalText = "";

            // TODO: Putter vi med vilje et delay ind?
            Thread.Sleep(_delay);

            if (new MySqlHandler().IsAvailable())
            {
                // Authorizing user
                LoadingText = "Connection established...";

                // TODO: Er den her nødvendig?
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
                CurrentActionText = "Unfortunately the excellent connection to the database was not established...";
                Reconnect();

                // Reconnect is required
                return true;
            }
        }

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

        private void LoadConfig()
        {
            throw new NotImplementedException();
        }
    }
}
