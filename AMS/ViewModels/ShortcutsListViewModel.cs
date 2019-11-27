using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.ViewModels
{
    public class ShortcutsListViewModel : Base.BaseViewModel
    {
        public Dictionary<string, Dictionary<string, string>> ShortcutsList { get; set; }

        // TODO: Fix loophole med at kunne tilgå ting man ikke må gennem shortcuts

        public ShortcutsListViewModel()
        {
            ShortcutsList = new Dictionary<string, Dictionary<string, string>>();

            ShortcutsList.Add("General", new Dictionary<string, string>());
            ShortcutsList["General"].Add("Alt + 1", "Home");
            ShortcutsList["General"].Add("Alt + 2", "Assets");
            ShortcutsList["General"].Add("Alt + 3", "Tags");
            ShortcutsList["General"].Add("Alt + 4", "Users");
            ShortcutsList["General"].Add("Alt + 5", "Log");

            ShortcutsList.Add("List view", new Dictionary<string, string>());
            ShortcutsList["List view"].Add("Delete / Ctrl + D ", "Delete item");
            ShortcutsList["List view"].Add("Enter / Ctrl + W", "View item");
            ShortcutsList["List view"].Add("Ctrl + E", "Edit item");
            ShortcutsList["List view"].Add("Crtl + P", "Export item(s)");
            ShortcutsList["List view"].Add("Ctrl + N", "Add new item");
            ShortcutsList["List view"].Add("Ctrl + F", "Search items");

            ShortcutsList.Add("Edit item", new Dictionary<string, string>());
            ShortcutsList["Edit item"].Add("Ctrl + S", "Save item / changes");
            ShortcutsList["Edit item"].Add("Ctrl + Shift + S", "Save copy of current item");
            ShortcutsList["Edit item"].Add("Ctrl + Q", "Cancel / Go back");

            ShortcutsList.Add("Asset view", new Dictionary<string, string>());
            ShortcutsList["Asset view"].Add("Ctrl + 1", "Asset details");
            ShortcutsList["Asset view"].Add("Ctrl + 2", "Asset comments");
            ShortcutsList["Asset view"].Add("Ctrl + 3", "Asset log");
            ShortcutsList["Asset view"].Add("Ctrl + Enter", "Save new comment");

            OnPropertyChanged(nameof(ShortcutsList));
        }
    }
}
