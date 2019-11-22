using AMS.Controllers;
using AMS.Database.Repositories;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.IO;
using AMS.Models;
using AMS.ViewModels;
using AMS.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace AMS.Helpers
{
    public static class PageMaker
    {
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

        #endregion

        /// <summary>
        /// Returns a new AssetPresenter page
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="tagables"></param>
        /// <returns></returns>
        public static Page CreateAssetPresenter(Asset asset, List<ITagable> tagables)
        {
            return new AssetPresenter(asset, tagables, new CommentListController(Features.GetCurrentSession(), _commentRepository));
        }

        /// <summary>
        /// Returns a new AssetEditor page
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static Page CreateAssetEditor(Asset asset = null)
        {
            return new AssetEditor(new AssetController(asset, _assetRepository), new TagListController(_printHelper));
        }

        /// <summary>
        /// Returns a new AssetList page
        /// </summary>
        /// <returns></returns>
        public static Page CreateAssetList()
        {
            return new AssetList(new AssetListController(_assetRepository, _printHelper));
        }

        /// <summary>
        /// Returns a new Home page
        /// </summary>
        /// <returns></returns>
        public static Page CreateHome()
        {
            return new Home();
        }

        /// <summary>
        /// Returns a new LogList page
        /// </summary>
        /// <returns></returns>
        public static Page CreateLogList()
        {
            return new LogList(new LogListController(_logRepository, _printHelper));
        }

        public static Page CreateTagEditor(Tag tag)
        {
            return new TagEditor(new TagController(tag, _tagRepository));
        }

        public static Page CreateTagList()
        {
            return new TagList(new TagListController(_printHelper));
        }

        /// <summary>
        /// Returns a new UserList page
        /// </summary>
        /// <returns></returns>
        public static Page CreateUserList()
        {
            return new UserList(new UserListController(_userImporter, _userRepository, _departmentRepository));
        }
    }
}
