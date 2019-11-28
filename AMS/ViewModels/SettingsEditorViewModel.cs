using AMS.Authentication;
using AMS.ConfigurationHandler;
using System;
using System.Windows.Input;

namespace AMS.ViewModels
{
    class SettingsEditorViewModel : Base.BaseViewModel
    {
        private string _oldPassword;
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public string IP { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } = String.Empty;
        public string Database { get; set; }
        public string Charset { get; set; }
        public string Timeout { get; set; }
        public bool SavePassword { get; set; } = false;

        public SettingsEditorViewModel()
        {
            string conString = Session.GetDBKey();
            string[] elements = conString.Split(';', '=');

            IP = elements[1];
            Username = elements[5];
            _oldPassword = elements[7];
            Database = elements[3];
            Charset = elements[9];
            Timeout = elements[11];

            SaveCommand = new Base.RelayCommand(Save);
            CancelCommand = new Base.RelayCommand(Cancel);
        }

        private void Cancel()
        {
            Features.Navigate.Back();
        }

        private void Save()
        {
            // Use the old password
            if (!SavePassword)
                Password = _oldPassword;

            string conString = $"Server={ IP }; database={ Database }; UID={ Username }; password={ Password }; Charset={ Charset }; Connect Timeout={ Timeout }";
            new FileConfigurationHandler(null).SetConfigValue(conString);
            Features.AddNotification(new Models.Notification("Settings saved", Models.Notification.APPROVE));
            Features.Navigate.Back();
        }
    }
}
