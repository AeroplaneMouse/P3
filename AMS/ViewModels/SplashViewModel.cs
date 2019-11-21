﻿using System;
using AMS.Database;
using System.Threading;
using AMS.Authentication;
using System.Windows.Input;
using System.Threading.Tasks;
using AMS.Database.Repositories;

namespace AMS.ViewModels
{
    class SplashViewModel : Base.BaseViewModel
    {
        private MainViewModel _main;
        private const int _delay = 300;
        private const int _reconnectWaitingTime = 5;

        public string LoadingText { get; set; }
        public string CurrentActionText { get; set; }
        public string AdditionalText { get; set; }
        public ICommand LoadConfigCommand { get; set; }


        public SplashViewModel(MainViewModel main)
        {
            Console.WriteLine("Showing splash screen");
            _main = main;

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

                Session t = new Session(new UserRepository());
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