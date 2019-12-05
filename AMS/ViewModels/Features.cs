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
        public static MainViewModel Main;
        public static Visibility OnlyVisibleForAdmin => Main.OnlyVisibleForAdmin;

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

        #region Helpers

        private static IExporter _printHelper = new PrintHelper();

        private static IUserImporter _userImporter = new UserImporter(UserRepository);

        private static TagHelper CreateTagHelper() => new TagHelper(TagRepository, UserRepository);

        #endregion

        // Notifications
        public static void AddNotification(Notification n, int displayTime = 2500) => Main.AddNotification(n, displayTime);

        // Prompts
        public static void DisplayPrompt(Page prompt) => Main.DisplayPrompt(prompt);

        // Session
        public static Session GetCurrentSession() => Main.CurrentSession;

        // Current department
        public static Department GetCurrentDepartment() => Main.CurrentDepartment;

        // Navigation
        public static class Navigate
        {
            private static Page _currentPage;

            public static bool To(Page page)
            {
                if (Main.ContentFrame.Navigate(page))
                {
                    if (_currentPage == null)
                    {
                        _currentPage = page;

                        return true;
                    }

                    if (page.GetType() == typeof(Home) || 
                        page.GetType() == typeof(AssetList) || 
                        page.GetType() == typeof(TagList) ||
                        page.GetType() == typeof(UserList) ||
                        page.GetType() == typeof(LogList))
                    {
                        Main.History.Clear();
                    }

                    Main.History.Push(_currentPage);
                    _currentPage = page;


                    return true;
                }

                return false;
            }

            public static bool Back()
            {
                if (Main.History.Count > 0)
                {
                    _currentPage = Main.History.Pop();

                    if (_currentPage.GetType() == typeof(Home)      ||
                        _currentPage.GetType() == typeof(AssetList) ||
                        _currentPage.GetType() == typeof(TagList)   ||
                        _currentPage.GetType() == typeof(UserList)  ||
                        _currentPage.GetType() == typeof(LogList))
                    {
                        Main.History.Clear();
                    }

                    (_currentPage.DataContext as IPageUpdateOnFocus).UpdateOnFocus();
                    Main.ContentFrame.Navigate(_currentPage);
                    return true;
                }

                return false;
            }
        }

        // Page and window creation
        public static class Create
        {
            #region Pages

            /// <summary>
            /// Returns a new AssetPresenter page
            /// </summary>
            /// <param name="asset"></param>
            /// <param name="tagables"></param>
            /// <returns></returns>
            public static Page AssetPresenter(Asset asset, List<ITagable> tagables)
            {
                // Create new asset if null
                asset = asset ?? new Asset();

                return new AssetPresenter(tagables, new AssetController(asset, AssetRepository), new CommentListController(GetCurrentSession(), CommentRepository, asset), new LogListController(LogRepository, _printHelper, asset));
            }

            /// <summary>
            /// Returns a new AssetEditor page
            /// </summary>
            /// <param name="asset"></param>
            /// <returns></returns>
            public static Page AssetEditor(Asset asset = null)
            {
                return new AssetEditor(new AssetController(asset ?? new Asset(), AssetRepository), CreateTagHelper());
            }

            /// <summary>
            /// Returns a new AssetEditor page
            /// </summary>
            /// <param name="asset"></param>
            /// <returns></returns>
            public static Page AssetEditor(IAssetController controller)
            {
                return new AssetEditor(controller, CreateTagHelper());
            }

            /// <summary>
            /// Returns a new AssetList page
            /// </summary>
            /// <returns></returns>
            public static Page AssetList()
            {
                return new AssetList(new AssetListController(AssetRepository, _printHelper), CreateTagHelper());
            }

            /// <summary>
            /// Returns a new Home page
            /// </summary>
            /// <returns></returns>
            public static Page Home()
            {
                return new Home(new HomeController(UserRepository, AssetRepository, TagRepository, DepartmentRepository), new CommentListController(GetCurrentSession(), CommentRepository));
            }

            public static Page ShortcutsList()
            {
                return new ShortcutsList();
            }

            /// <summary>
            /// Returns a new Splash page
            /// </summary>
            /// <param name="main"></param>
            /// <returns></returns>
            public static Page Splash()
            {
                return new Splash(Main, UserRepository);
            }

            /// <summary>
            /// Returns a new LogList page
            /// </summary>
            /// <returns></returns>
            public static Page LogList()
            {
                return new LogList(new LogListController(LogRepository, _printHelper));
            }

            /// <summary>
            /// Returns a new LogPresenter page
            /// </summary>
            /// <param name="entry"></param>
            /// <returns></returns>
            public static Page LogPresenter(LogEntry entry)
            {
                return new LogPresenter(entry);
            }

            /// <summary>
            /// Returns a new TagEditor page
            /// </summary>
            /// <param name="tag"></param>
            /// <returns></returns>
            public static Page TagEditor(Tag tag)
            {
                return new TagEditor(new TagController(tag ?? new Tag(), TagRepository, DepartmentRepository));
            }

            /// <summary>
            /// Returns a new TagList page
            /// </summary>
            /// <returns></returns>
            public static Page TagList()
            {
                return new TagList(new TagListController(TagRepository, _printHelper), new TagController(new Tag(), TagRepository, DepartmentRepository));
            }

            /// <summary>
            /// Returns a new UserList page
            /// </summary>
            /// <returns></returns>
            public static Page UserList()
            {
                return new UserList(new UserListController(_userImporter, UserRepository, DepartmentRepository));
            }
            
            /// <summary>
            /// Returns a new settings editor page
            /// </summary>
            /// <returns></returns>
            public static Page SettingsEditor(object caller)
            {
                return new SettingsEditor(caller);
            }

            #endregion

            #region Windows

            /// <summary>
            /// Returns the main window of the application
            /// </summary>
            /// <returns></returns>
            public static Window MainWindow()
            {
                return new Main(UserRepository, DepartmentRepository);
            }

            #endregion
        }
    }    
}
