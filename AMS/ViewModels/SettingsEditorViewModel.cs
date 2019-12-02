using AMS.Authentication;
using AMS.ConfigurationHandler;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace AMS.ViewModels
{
    class SettingsEditorViewModel : Base.BaseViewModel
    {
        private string _oldPassword;
        private object _caller;

        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public string IP { get; set; } = String.Empty;
        public string Username { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string Database { get; set; } = String.Empty;
        public string Charset { get; set; } = "utf8";
        public string Timeout { get; set; } = "5";

        public SettingsEditorViewModel(object caller)
        {
            _caller = caller;
            UpdateOnFocus();
        }

        private void Cancel()
        {
            // Return from splashpage if called from it.
            if (_caller is SplashViewModel)
                Features.Main.SplashPage = Features.Create.Splash();
            else
                Features.Navigate.Back();
        }

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

        public override void UpdateOnFocus()
        {
            string conString = Session.GetDBKey();

            // If a current configuration exists, load it to the view.
            if (!String.IsNullOrEmpty(conString))
            {
                string[] elements = conString.Split(';', '=');

                IP = elements[1];
                Username = elements[5];
                _oldPassword = elements[7];
                Database = elements[3];
                Charset = elements[9];
                Timeout = elements[11];
            }

            SaveCommand = new Base.RelayCommand(Save);
            CancelCommand = new Base.RelayCommand(Cancel);
        }
    }
}
