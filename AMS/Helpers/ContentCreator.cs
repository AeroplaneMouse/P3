using AMS.Controllers;
using AMS.Controllers.Interfaces;
using AMS.Interfaces;
using AMS.IO;
using AMS.Logging;
using AMS.Models;
using AMS.ViewModels;
using AMS.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AMS.Helpers
{
    public class ContentCreator
    {
        #region Helpers

        private static IExporter _printHelper { get; } = new PrintHelper();
        private static IUserImporter _userImporter { get; } = new UserImporter(Features.UserRepository);
        private static TagHelper CreateTagHelper() => new TagHelper(Features.TagRepository, Features.UserRepository);

        #endregion

        #region Controllers

        private IAssetController GetAssetController(Asset asset) => new AssetController(asset, Features.AssetRepository, Features.GetCurrentSession());

        private IAssetListController GetAssetListController() => new AssetListController(Features.AssetRepository, _printHelper);

        private ICommentListController GetCommentListController(Asset asset) => 
            new CommentListController(Features.GetCurrentSession(), Features.CommentRepository, Features.GetCurrentDepartment(), asset);

        private ILogListController GetLogListController(Asset asset) => new LogListController(Features.LogRepository, _printHelper, asset);

        private IHomeController GetHomeController() => 
            new HomeController(Features.UserRepository, Features.AssetRepository, Features.TagRepository, Features.DepartmentRepository);

        private ITagController GetTagController(Tag tag) => new TagController(tag, Features.TagRepository, Features.DepartmentRepository);

        private ITagListController GetTagListController() => new TagListController(Features.TagRepository);

        private IUserListController GetUserListController() => new UserListController(_userImporter, Features.UserRepository, Features.DepartmentRepository);

        #endregion

        #region Pages

        /// <summary>
        /// Returns a new AssetPresenter page
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="tagables"></param>
        /// <returns></returns>
        public Page AssetPresenter(Asset asset, List<ITagable> tagables)
        {
            return new AssetPresenter(tagables, GetAssetController(asset), GetCommentListController(asset), GetLogListController(asset));
        }

        /// <summary>
        /// Returns a new AssetEditor page
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public Page AssetEditor(Asset asset = null)
        {
            return new AssetEditor(GetAssetController(asset ?? new Asset()), CreateTagHelper());
        }

        /// <summary>
        /// Returns a new AssetEditor page
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public Page AssetEditor(IAssetController controller)
        {
            return new AssetEditor(controller, CreateTagHelper());
        }

        /// <summary>
        /// Returns a new AssetList page
        /// </summary>
        /// <returns></returns>
        public Page AssetList()
        {
            return new AssetList(GetAssetListController(), CreateTagHelper());
        }

        /// <summary>
        /// Returns a new Home page
        /// </summary>
        /// <returns></returns>
        public Page Home()
        {
            return new Home(GetHomeController(), GetCommentListController(null));
        }

        public Page ShortcutsList()
        {
            return new ShortcutsList();
        }

        /// <summary>
        /// Returns a new Splash page
        /// </summary>
        /// <param name="main"></param>
        /// <returns></returns>
        public Page Splash()
        {
            return new Splash(Features.Main, Features.UserRepository);
        }

        /// <summary>
        /// Returns a new LogList page
        /// </summary>
        /// <returns></returns>
        public Page LogList()
        {
            return new LogList(GetLogListController(null));
        }

        /// <summary>
        /// Returns a new LogPresenter page
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public Page LogPresenter(LogEntry entry)
        {
            return new LogPresenter(entry);
        }

        /// <summary>
        /// Returns a new TagEditor page
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Page TagEditor(Tag tag)
        {
            return new TagEditor(GetTagController(tag ?? new Tag()));
        }

        /// <summary>
        /// Returns a new TagList page
        /// </summary>
        /// <returns></returns>
        public Page TagList()
        {
            return new TagList(GetTagListController(), GetTagController(new Tag()));
        }

        /// <summary>
        /// Returns a new UserList page
        /// </summary>
        /// <returns></returns>
        public Page UserList()
        {
            return new UserList(GetUserListController());
        }

        /// <summary>
        /// Returns a new settings editor page
        /// </summary>
        /// <returns></returns>
        public Page SettingsEditor(object caller)
        {
            return new SettingsEditor(caller);
        }

        #endregion

        #region Windows

        /// <summary>
        /// Returns the main window of the application
        /// </summary>
        /// <returns></returns>
        public Window MainWindow()
        {
            return new Main(Features.UserRepository, Features.DepartmentRepository);
        }

        #endregion
    }
}
