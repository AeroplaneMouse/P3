using System;
using System.Threading;
using System.Windows.Input;
using System.Threading.Tasks;
using Asset_Management_System.Events;
using Asset_Management_System.Database;
using Asset_Management_System.Authentication;
using System.Windows.Threading;
using Asset_Management_System.Services.Interfaces;

namespace Asset_Management_System.ViewModels
{
    public class SplashViewModel : Base.BaseViewModel
    {
        private MainViewModel _main;
        private IUserService _userService;
        private const int _delay = 300;
        private const int _reconnectWaitingTime = 5;

        public string LoadingText { get; set; }
        public string CurrentActionText { get; set; }
        public string AdditionalText { get; set; }

        public ICommand LoadConfigCommand { get; set; }


        public SplashViewModel(MainViewModel main, IUserService userService)
        {
            Console.WriteLine("Showing splash screen");
            _main = main;
            _userService = userService;

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
            CurrentActionText = "An excellent connection to the database is being established...";
            AdditionalText = "";
            Thread.Sleep(_delay);

            if (new MySqlHandler().IsAvailable())
            {
                // Authorizing user
                LoadingText = "Connection established...";
                CurrentActionText = "The excellent connection to the database was succesfully established...";
                Thread.Sleep(_delay);

                Session t = new Session(_userService);
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

                return false;
            }
            else
            {
                LoadingText = "ERROR!";
                CurrentActionText = "Unfortunately the excellent connection to the database was not established...";
                Reconnect();
            }

            return true;
        }

        private void Reconnect()
        {
            const string baseText = "Reconnecting in";
            for(int i = _reconnectWaitingTime; i > 0; i--)
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