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

namespace AMS.ViewModels
{
    static class Features
    {
        public static MainViewModel Main;
        public static Visibility OnlyVisibleForAdmin => Main.OnlyVisibleForAdmin;

        #region Repositories

        private static IUserRepository _userRepository = new UserRepository();

        private static IAssetRepository _assetRepository = new AssetRepository();

        private static ITagRepository _tagRepository = new TagRepository();

        private static IDepartmentRepository _departmentRepository = new DepartmentRepository();

        private static ICommentRepository _commentRepository = new CommentRepository();

        private static ILogRepository _logRepository = new LogRepository();

        #endregion

        #region Helpers

        private static IExporter _printHelper = new PrintHelper();

        private static IUserImporter _userImporter = new UserImporter(_userRepository);

        private static TagHelper CreateTagHelper(ObservableCollection<ITagable> tags = null)
        {
            return new TagHelper(_tagRepository, _userRepository, tags ?? new ObservableCollection<ITagable>());
        }

        #endregion

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
                return new AssetPresenter(asset, tagables, new CommentListController(Features.GetCurrentSession(), _commentRepository));
            }

            /// <summary>
            /// Returns a new AssetEditor page
            /// </summary>
            /// <param name="asset"></param>
            /// <returns></returns>
            public static Page AssetEditor(Asset asset = null)
            {
                return new AssetEditor(new AssetController(asset, _assetRepository), new TagListController(_tagRepository, _printHelper));
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
            /// Returns a new TagEditor page
            /// </summary>
            /// <param name="tag"></param>
            /// <returns></returns>
            public static Page TagEditor(Tag tag)
            {
                return new TagEditor(new TagController(tag, _tagRepository));
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
