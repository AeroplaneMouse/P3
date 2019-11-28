using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using AMS.Authentication;
using AMS.Controllers;
using AMS.Database.Repositories;
using AMS.Database.Repositories.Interfaces;
using AMS.Interfaces;
using AMS.IO;
using AMS.Models;
using AMS.ViewModels;
using AMS.Views;

namespace AMS.Helpers.Features
{
    // Page and window creation
        public class Create
        {
            private IAssetRepository _assetRepository;
            private ICommentRepository _commentRepository;
            private IDepartmentRepository _departmentRepository;
            private ILogRepository _logRepository;
            private IExporter _printHelper;
            private ITagRepository _tagRepository;
            private IUserRepository _userRepository;
            private IUserImporter _userImporter;

            public Create()
            {
                _assetRepository = new AssetRepository();
                _commentRepository = new CommentRepository();
                _departmentRepository = new DepartmentRepository();
                _logRepository = new LogRepository();
                _printHelper = new PrintHelper();
                _tagRepository = new TagRepository();
                _userRepository = new UserRepository();
                _userImporter = new UserImporter(_userRepository);
            }
            
            private TagHelper CreateTagHelper()
            {
                return new TagHelper(_tagRepository, _userRepository);
            }

            
            
            #region Pages

            /// <summary>
            /// Returns a new AssetPresenter page
            /// </summary>
            /// <param name="asset"></param>
            /// <param name="tagables"></param>
            /// <returns></returns>
            public Page AssetPresenter(Asset asset, List<ITagable> tagables)
            {
                return new AssetPresenter(tagables, 
                    new AssetController(asset, _assetRepository), 
                    new CommentListController(Features.Instance.GetCurrentSession(), _commentRepository, asset), 
                    new LogListController(_logRepository, _printHelper));
            }

            /// <summary>
            /// Returns a new AssetEditor page
            /// </summary>
            /// <param name="asset"></param>
            /// <returns></returns>
            public Page AssetEditor(Asset asset = null)
            {
                // Create a new asset if null
                if (asset == null)
                    asset = new Asset();

                return new AssetEditor(
                    new AssetController(asset, _assetRepository), 
                    new TagListController(_tagRepository, _printHelper), CreateTagHelper());
            }

            /// <summary>
            /// Returns a new AssetList page
            /// </summary>
            /// <returns></returns>
            public Page AssetList()
            {
                return new AssetList(
                    new AssetListController(_assetRepository, _printHelper), CreateTagHelper());
            }

            /// <summary>
            /// Returns a new Home page
            /// </summary>
            /// <returns></returns>
            public Page Home()
            {
                return new Home(_userRepository, _assetRepository, _tagRepository, _departmentRepository);
            }

            /// <summary>
            /// Returns a new Splash page
            /// </summary>
            /// <param name="main"></param>
            /// <returns></returns>
            public Page Splash(MainViewModel main)
            {
                return new Splash(main, _userRepository);
            }

            /// <summary>
            /// Returns a new LogList page
            /// </summary>
            /// <returns></returns>
            public Page LogList()
            {
                return new LogList(
                    new LogListController(_logRepository, _printHelper));
            }

            /// <summary>
            /// Returns a new TagEditor page
            /// </summary>
            /// <param name="tag"></param>
            /// <returns></returns>
            public Page TagEditor(Tag tag)
            {
                return new TagEditor(
                    new TagController(tag, _tagRepository, _departmentRepository));
            }

            /// <summary>
            /// Returns a new TagList page
            /// </summary>
            /// <returns></returns>
            public Page TagList()
            {
                return new TagList(new TagListController(_tagRepository, _printHelper));
            }

            /// <summary>
            /// Returns a new UserList page
            /// </summary>
            /// <returns></returns>
            public Page UserList()
            {
                return new UserList(
                    new UserListController(_userImporter, _userRepository, _departmentRepository));
            }

            #endregion

            #region Windows

            /// <summary>
            /// Returns the main window of the application
            /// </summary>
            /// <returns></returns>
            public Window MainWindow()
            {
                return new Main(_userRepository, _departmentRepository);
            }

            /// <summary>
            /// Creates and returns a new MainViewModel with the given window
            /// </summary>
            /// <param name="window"></param>
            /// <returns></returns>
            public MainViewModel CreateMainViewModel(Window window)
            {
                return new MainViewModel(window, _userRepository, _departmentRepository);
            }

            #endregion
        }
}