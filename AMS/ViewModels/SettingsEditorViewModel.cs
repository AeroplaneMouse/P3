using System;
using AMS.Authentication;
using System.Windows.Input;
using AMS.ConfigurationHandler;

namespace AMS.ViewModels
{
    class SettingsEditorViewModel : Base.BaseViewModel
    {
        private string _oldPassword;
        private object _caller;

        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand LoadFromFileCommand { get; set; }

        public string IP { get; set; } = String.Empty;
        public string Username { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string Database { get; set; } = String.Empty;
        public string Charset { get; set; } = "utf8";
        public string Timeout { get; set; } = "5";

        public SettingsEditorViewModel(object caller)
        {
            _caller = caller;

            SaveCommand = new Base.RelayCommand(Save);
            CancelCommand = new Base.RelayCommand(Cancel);
            LoadFromFileCommand = new Base.RelayCommand(LoadSettings);
        }

        /// <summary>
        /// Aborts the new configuration settings and returns to the previous page
        /// /// </summary>
        private void Cancel()
        {
            // Return from splashpage if called from it.
            if (_caller is SplashViewModel)
                Features.Main.SplashPage = Features.Create.Splash();
            else
                Features.Navigate.Back();
        }

        /// <summary>
        /// Save the current settings.
        /// </summary>
        private void Save()
        {
            // Use the old password
            if (String.IsNullOrEmpty(Password))
                Password = _oldPassword;

            string conString = $"Server={ IP }; database={ Database }; UID={ Username }; password={ Password }; Charset={ Charset }; Connect Timeout={ Timeout }";
            new FileConfigurationHandler(null).SetConfigValue(conString);
            Features.AddNotification(new Models.Notification("Settings saved", Models.Notification.APPROVE));
            Features.Main.Reload();
        }

        /// <summary>
        /// Load the settings when the page gets focus.
        /// </summary>
        public override void UpdateOnFocus()
        {
            string conString = Session.GetDBKey();

            // If a current configuration exists, load it to the view.
            if (!String.IsNullOrEmpty(conString))
                ExtractSettingsFromString(conString);
        }

        /// <summary>
        /// Extracts the configuration settings from the configuration string 
        /// and saves them to properties to be displayed on the view.
        /// </summary>
        /// <param name="connectionString"></param>
        private void ExtractSettingsFromString(string connectionString)
        {
            string[] elements = connectionString.Split(';', '=');

            IP = elements[1];
            Username = elements[5];
            _oldPassword = elements[7];
            Database = elements[3];
            Charset = elements[9];
            Timeout = elements[11];
        }

        /// <summary>
        /// Reads the configuration settings from a user seleted file, then loads these settings into the view to be displayed.
        /// </summary>
        private void LoadSettings()
        {
            string path = GetFilePath();
            if (!String.IsNullOrEmpty(path))
            {
                // Load settings from file and save them to local config file
                FileConfigurationHandler configurationhandler = new FileConfigurationHandler(Features.GetCurrentSession());
                string conString  = configurationhandler.LoadConfigValueFromExternalFile(path);
                ExtractSettingsFromString(conString);
                Features.AddNotification(new Models.Notification("Settings have been loaded...", Models.Notification.INFO), displayTime: 3000);
            }
        }

        /// <summary>
        /// Opens a dialog for the user to select a file.
        /// </summary>
        /// <returns>The selected filepath</returns>
        public string GetFilePath()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = dialog.ShowDialog();
            if (result == false)
                return String.Empty;

            return dialog.FileName;
        }
    }
}
