using AMS.Controllers.Interfaces;
using AMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace AMS.ViewModels
{
    public class CommentViewModel : Base.BaseViewModel
    {
        private Asset _asset { get; set; }
        public string NewComment { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ObservableCollection<Comment> CommentList => new ObservableCollection<Comment>(_controller.CommentList);
        private ICommentListController _controller { get; set; }

        public CommentViewModel(Asset asset, ICommentListController commentListController)
        {
            _asset = asset;
            _controller = commentListController;

            SaveCommand = new Base.RelayCommand(SaveComment);
            DeleteCommand = new Base.RelayCommand<object>(DeleteComment);
        }

        public override void UpdateOnFocus()
        {
            OnPropertyChanged(nameof(CommentList));
        }

        private void SaveComment()
        {
            NewComment = NewComment.Trim();
            if (!string.IsNullOrEmpty(NewComment))
                _controller.AddNewComment(NewComment);
            NewComment = string.Empty;
            OnPropertyChanged(nameof(CommentList));
        }

        private void DeleteComment(object comment)
        {
            _controller.RemoveComment(comment as Comment);
            Features.AddNotification(new Notification("Comment was removed", Notification.INFO));
            OnPropertyChanged(nameof(CommentList));
        }
    }
}
