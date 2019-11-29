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

        private static IUserRepository _userRepository { get; } = new UserRepository();
        private static IAssetRepository _assetRepository { get; } = new AssetRepository();
        private static ITagRepository _tagRepository { get; } = new TagRepository();
        private static IDepartmentRepository _departmentRepository { get; } = new DepartmentRepository();
        private static ICommentRepository _commentRepository { get; } = new CommentRepository();
        private static ILogRepository _logRepository { get; } = new LogRepository();

        #endregion

        #region Helpers

        private static IExporter _printHelper = new PrintHelper();

        private static IUserImporter _userImporter = new UserImporter(_userRepository);

        private static TagHelper CreateTagHelper()
        {
            return new TagHelper(_tagRepository, _userRepository);
        }

        #endregion

        // Notifications
        public static void AddNotification(Notification n, int displayTime = 2500)
        {
            Main.AddNotification(n, displayTime);
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
                return new AssetPresenter(tagables, new AssetController(asset, _assetRepository), new CommentListController(GetCurrentSession(), _commentRepository, asset), new LogListController(_logRepository, _printHelper, asset));
            }

            /// <summary>
            /// Returns a new AssetEditor page
            /// </summary>
            /// <param name="asset"></param>
            /// <returns></returns>
            public static Page AssetEditor(Asset asset = null)
            {
                // Create a new asset if null
                if (asset == null)
                    asset = new Asset();

                return new AssetEditor(new AssetController(asset, _assetRepository), CreateTagHelper());
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
                return new AssetList(new AssetListController(_assetRepository, _printHelper), CreateTagHelper());
            }

            /// <summary>
            /// Returns a new Home page
            /// </summary>
            /// <returns></returns>
            public static Page Home()
            {
                return new Home(_userRepository, _assetRepository, _tagRepository, _departmentRepository);
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
            public static Page Splash(MainViewModel main)
            {
                return new Splash(main, _userRepository);
            }

            /// <summary>
            /// Returns a new LogList page
            /// </summary>
            /// <returns></returns>
            public static Page LogList()
            {
                return new LogList(new LogListController(_logRepository, _printHelper));
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
                return new TagEditor(new TagController(tag, _tagRepository, _departmentRepository));
            }

            /// <summary>
            /// Returns a new TagList page
            /// </summary>
            /// <returns></returns>
            public static Page TagList()
            {
                return new TagList(new TagListController(_tagRepository, _printHelper));
            }

            /// <summary>
            /// Returns a new UserList page
            /// </summary>
            /// <returns></returns>
            public static Page UserList()
            {
                return new UserList(new UserListController(_userImporter, _userRepository, _departmentRepository));
            }
            
            /// <summary>
            /// Returns a new settings editor page
            /// </summary>
            /// <returns></returns>
            public static Page SettingsEditor()
            {
                return new SettingsEditor();
            }

            #endregion

            #region Windows

            /// <summary>
            /// Returns the main window of the application
            /// </summary>
            /// <returns></returns>
            public static Window Main()
            {
                return new Main(_userRepository, _departmentRepository);
            }

            #endregion
        }
    }    
}
