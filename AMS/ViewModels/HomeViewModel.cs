using AMS.Models;
using System.Windows.Input;
using System.Collections.Generic;
using AMS.Controllers.Interfaces;

namespace AMS.ViewModels
{
    public class HomeViewModel : Base.BaseViewModel
    {
        private IHomeController _homeController { get; set; }
        private ICommentListController _commentListController { get; set; }

        public ulong NumberOfUsers => _homeController.NumberOfUsers;
        public ulong NumberOfAssets => _homeController.NumberOfAssets;
        public ulong NumberOfTags => _homeController.NumberOfTags;
        public ulong NumberOfDepartments => _homeController.NumberOfDepartments;
        public List<Comment> CommentList => _commentListController.CommentList;
        public Comment SelectedComment { get; set; }
        public string CurrentDepartment => "(" + Features.GetCurrentDepartment().Name + ")";


        public ICommand ViewCommand { get; set; }
        

        /// <summary>
        /// Default contructor
        /// </summary>
        public HomeViewModel(IHomeController homeController, ICommentListController commentListController)
        {
            _homeController = homeController;
            _commentListController = commentListController;

            ViewCommand = new Base.RelayCommand(View);

            // Notify view
            UpdateOnFocus();
        }

        public override void UpdateOnFocus()
        {
            OnPropertyChanged(nameof(NumberOfUsers));
            OnPropertyChanged(nameof(NumberOfAssets));
            OnPropertyChanged(nameof(NumberOfTags));
            OnPropertyChanged(nameof(NumberOfDepartments));
            OnPropertyChanged(nameof(CurrentDepartment));

            _commentListController.FetchComments();
            OnPropertyChanged(nameof(CommentList));
        }

        // Opens the AssetView for the currently selected commment
        public void View()
        {
            if (SelectedComment != null)
            {
                var asset = _homeController.GetAsset(SelectedComment.AssetID);
                Features.Navigate.To(Features.Create.AssetPresenter(asset, _homeController.GetTags(asset), 1));
            }
        }
    }
}