using AMS.Models;
using AMS.Authentication;
using System.Windows.Controls;
using System.Windows;
using AMS.Database.Repositories.Interfaces;
using AMS.Database.Repositories;
using AMS.Interfaces;
using AMS.Helpers;
using AMS.IO;
using System.Collections.ObjectModel;
using AMS.Controllers;
using AMS.Views;
using System.Collections.Generic;
using AMS.Logging;
using AMS.Controllers.Interfaces;

namespace AMS.ViewModels
{
    public static class Features
    {
        public static MainViewModel Main { get; set; }

        // Navigating between pages
        private static PageNavigator _navigator;
        public static PageNavigator Navigate => _navigator ??= new PageNavigator();

        // Page and window creation
        private static ContentCreator _creator;
        public static ContentCreator Create => _creator ??= new ContentCreator();
        
        #region Repositories

        private static IUserRepository _userRepository;
        private static IAssetRepository _assetRepository;
        private static ITagRepository _tagRepository;
        private static IDepartmentRepository _departmentRepository;
        private static ICommentRepository _commentRepository;
        private static ILogRepository _logRepository;

        public static IUserRepository UserRepository => _userRepository ??= new UserRepository();
        public static IAssetRepository AssetRepository => _assetRepository ??= new AssetRepository();
        public static ITagRepository TagRepository => _tagRepository ??= new TagRepository();
        public static IDepartmentRepository DepartmentRepository => _departmentRepository ??= new DepartmentRepository();
        public static ICommentRepository CommentRepository => _commentRepository ??= new CommentRepository();
        public static ILogRepository LogRepository => _logRepository ??= new LogRepository();

        #endregion

        // Whether or not a UI element is only visible for the Admin user
        public static Visibility OnlyVisibleForAdmin => Main.OnlyVisibleForAdmin;

        // Notifications
        public static void AddNotification(Notification n, int displayTime = 2500) => Main.AddNotification(n, displayTime);

        // Prompts
        public static void DisplayPrompt(Page prompt) => Main.DisplayPrompt(prompt);

        // Session
        public static Session GetCurrentSession() => Main.CurrentSession;

        // Current department
        public static Department GetCurrentDepartment() => Main.CurrentDepartment;

        // Sets the Main's splash page to be the splash page
        public static void SetSplashPage() => Main.SplashPage = Create.Splash();

        // Reload the system
        public static void ReloadAll() => Main.Reload();
    }
}