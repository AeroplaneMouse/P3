using System;
using System.Text;
using System.Threading;
using System.Windows.Input;
using System.Collections.Generic;
using Asset_Management_System.Events;
using Asset_Management_System.Database;
using Asset_Management_System.Authentication;
using System.ComponentModel;
using MySql.Data.MySqlClient;

namespace Asset_Management_System.ViewModels
{
    public class SplashViewModel : Base.BaseViewModel
    {
        private MainViewModel _main;
        private BackgroundWorker worker;
        public string LoadingText { get; set; }
        public string StatusText { get; set; }

        public ICommand LoadConfigCommand { get; set; }


        public SplashViewModel(MainViewModel main)
        {
            _main = main;

            // Initializing commands
            LoadConfigCommand = new Base.RelayCommand(() => LoadConfig());

            Console.WriteLine("Showing splash screen");
            Authenticate();
        }

        private void LoadConfig()
        {
            throw new NotImplementedException();
        }

        private void Authenticate()
        {
            UpdateStatusText(new StatusUpdateEventArgs("Loading...", "Starting background worker..."));

            // Initializing backgroundworker
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = false;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            // Starting backgroundworker
            worker.RunWorkerAsync();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Result is the active session, with the authorized user.
            Session t = e.Result as Session;
            if (t != null)
                _main.SystemLoaded(t);

            //Dispose the background after use.
            worker.Dispose();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateStatusText(e.UserState as StatusUpdateEventArgs);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker caller = sender as BackgroundWorker;

            // Check database connection
            caller.ReportProgress(0, new StatusUpdateEventArgs("Loading...", "Connecting to database."));

            if (new MySqlHandler().IsAvailable())
            {
                Session t = new Session();
                if (t.Authenticated())
                    e.Result = t;
                else
                    caller.ReportProgress(0, new StatusUpdateEventArgs("!!! Access denied !!!", $"User \"{ Session.GetIdentity() }\" is not authorized to access the application."));
            }
            else
            {
                caller.ReportProgress(0, new StatusUpdateEventArgs("ERROR!", "Error! Unable to connect to database."));
            }
        }

        public void Reload()
        {
            // Showing the splash page again
            _main.SplashVisibility = System.Windows.Visibility.Visible;
            _main.OnPropertyChanged(nameof(_main.SplashVisibility));
            
            Authenticate();
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

    }
}
