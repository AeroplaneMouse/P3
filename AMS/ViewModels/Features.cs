using AMS.Models;
using AMS.Authentication;
using System.Windows.Controls;

namespace AMS.ViewModels
{
    static class Features
    {
        public static MainViewModel Main;

        // Notifications
        public static void AddNotification(Notification n, int displayTime = 2500)
        {
            Main.AddNotification(n, displayTime);
        }

        // Navigation
        public static bool NavigatePage(Page page)
        {
            return Main.ContentFrame.Navigate(page);
        }

        public static bool NavigateBack()
        {
            if (Main.ContentFrame.CanGoBack)
            {
                Main.ContentFrame.GoBack();
                return true;
            }
            else
                return false;
        }

        // Prompts
        public static void DisplayPrompt(Page prompt)
        {
            Main.DisplayPrompt(prompt);
        }

        // Session
        public static Session GetCurrentSession()
        {
            return Main.CurrentSession;
        }
    }
}
