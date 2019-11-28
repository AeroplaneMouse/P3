using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.ViewModels
{
    public class ShortcutsListViewModel : Base.BaseViewModel
    {
        public Dictionary<string, Dictionary<string, string>> ShortcutsList { get; set; }

        // TODO: Gør mig pæn

        public ShortcutsListViewModel()
        {
            ShortcutsList = new Dictionary<string, Dictionary<string, string>>();

            ShortcutsList.Add("General", new Dictionary<string, string>());
            ShortcutsList["General"].Add("Alt + 1", "Home");
            ShortcutsList["General"].Add("Alt + 2", "Assets");
            ShortcutsList["General"].Add("Alt + 3", "Tags");
            ShortcutsList["General"].Add("Alt + 4", "Users");
            ShortcutsList["General"].Add("Alt + 5", "Log");

            ShortcutsList.Add("Overview", new Dictionary<string, string>());
            ShortcutsList["Overview"].Add("Delete / Ctrl + D ", "Delete item");
            ShortcutsList["Overview"].Add("Enter / Ctrl + W", "View item");
            ShortcutsList["Overview"].Add("Ctrl + E", "Edit item");
            ShortcutsList["Overview"].Add("Crtl + P", "Export item(s)");
            ShortcutsList["Overview"].Add("Ctrl + N", "Add new item");
            //ShortcutsList["List view"].Add("Ctrl + F", "Search items");

            ShortcutsList.Add("Edit item", new Dictionary<string, string>());
            ShortcutsList["Edit item"].Add("Ctrl + S", "Save item / Changes");
            ShortcutsList["Edit item"].Add("Ctrl + Shift + S", "Save copy of current item");
            ShortcutsList["Edit item"].Add("Esc / Ctrl + Q", "Cancel / Go back");

            ShortcutsList.Add("Asset inspection", new Dictionary<string, string>());
            ShortcutsList["Asset inspection"].Add("Ctrl + Enter", "Save new comment");

            OnPropertyChanged(nameof(ShortcutsList));
        }
    }
}
