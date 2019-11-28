using System.Windows;
using System.Windows.Controls;
using AMS.Authentication;
using AMS.Models;
using AMS.ViewModels;

namespace AMS.Helpers.Features
{
    public sealed class Features
    {
        public static readonly Features Instance = new Features();
        public Create Create { get; }

        private Navigate _navigate;
        public Navigate Navigate
        {
            get => _navigate ?? new Navigate(Main);
            set => _navigate = value;
        }

        // TODO: This needs to be set when the constructor is run.
        public MainViewModel Main { get; set; }
        public Visibility OnlyVisibleForAdmin => Main.OnlyVisibleForAdmin;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Features()
        {
        }

        private Features()
        {
            Create = new Create();
        }
        
        // Notifications
        public void AddNotification(Notification n, int displayTime = 2500)
        {
            Main.AddNotification(n, displayTime);
        }

        // Prompts
        public void DisplayPrompt(Page prompt)
        {
            Main.DisplayPrompt(prompt);
        }

        // Session
        public Session GetCurrentSession()
        {
            return Main.CurrentSession;
        }
    }
}