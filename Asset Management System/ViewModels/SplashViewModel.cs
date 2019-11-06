using System;
using System.Threading;
using System.Windows.Input;
using System.Threading.Tasks;
using Asset_Management_System.Events;
using Asset_Management_System.Database;
using Asset_Management_System.Authentication;
using System.Windows.Threading;

namespace Asset_Management_System.ViewModels
{
    public class SplashViewModel : Base.BaseViewModel
    {
        private MainViewModel _main;
        private const int _delay = 800;

        public string LoadingText { get; set; }
        public string StatusText { get; set; }

        public ICommand LoadConfigCommand { get; set; }


        public SplashViewModel(MainViewModel main)
        {
            Console.WriteLine("Showing splash screen");
            _main = main;

            // Initializing commands
            LoadConfigCommand = new Base.RelayCommand(() => LoadConfig());

            Setup();
        }

        private async void Setup()
        {
            UpdateStatusText(new StatusUpdateEventArgs("Loading...", "Initializing background worker..."));
            await Task.Run(Authenticate);
        }

        private void Authenticate()
        {
            // Connecting to database
            UpdateStatusText(new StatusUpdateEventArgs("Establishing connection...", "An excellent connection to the database is being established..."));
            Thread.Sleep(_delay);

            if (new MySqlHandler().IsAvailable())
            {
                // Authorizing user
                UpdateStatusText(new StatusUpdateEventArgs("Connection established...", "The excellent connection to the database was succesfully established..."));
                Thread.Sleep(_delay);

                Session t = new Session();
                if (t.Authenticated())
                {
                    UpdateStatusText(new StatusUpdateEventArgs("User authenticated", "Unlocking the Asset Management System..."));
                    Thread.Sleep(_delay);

                    _main.SystemLoaded(t);
                }
                else
                    UpdateStatusText(new StatusUpdateEventArgs(
                        "!!! Access denied !!!",
                        $"User \"{Session.GetIdentity()}\" is not authorized to access the application.")
                    );
            }
            else
                UpdateStatusText(new StatusUpdateEventArgs("Error!", "Unfortunately the excellent connection to the database was not established..."));
        }

        /// <summary>
        /// Updates text on the screen with progress and status.
        /// </summary>
        /// <param name="e"></param>
        public void UpdateStatusText(StatusUpdateEventArgs e)
        {
            LoadingText = e.Title;
            StatusText = e.Message;
            OnPropertyChanged(nameof(LoadingText));
            OnPropertyChanged(nameof(StatusText));
        }

        private void LoadConfig()
        {
            throw new NotImplementedException();
        }
    }
}