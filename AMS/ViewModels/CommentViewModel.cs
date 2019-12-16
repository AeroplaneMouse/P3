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
        public ICommand SaveCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand UpdateCommand{ get; set; }
        public ObservableCollection<Comment> CommentList => new ObservableCollection<Comment>(_controller.CommentList);
        private ICommentListController _controller { get; set; }
        public string NewComment { get; set; }

        public CommentViewModel(ICommentListController commentListController)
        {
            _controller = commentListController;

            SaveCommand = new Base.RelayCommand(SaveComment);
            DeleteCommand = new Base.RelayCommand<object>(DeleteComment);
            EditCommand = new Base.RelayCommand<object>(EditComment);
            UpdateCommand = new Base.RelayCommand<object>(UpdateComment);
        }

        public override void UpdateOnFocus()
        {
            OnPropertyChanged(nameof(CommentList));
        }

        /// <summary>
        /// Saves the text written in NewComment as a new comment
        /// </summary>
        private void SaveComment()
        {
            NewComment = NewComment.Trim();

            if (!string.IsNullOrEmpty(NewComment))
                _controller.AddNewComment(NewComment);

            NewComment = string.Empty;
            OnPropertyChanged(nameof(CommentList));
        }

        /// <summary>
        /// Deletes the input comment and informs the user 
        /// </summary>
        /// <param name="comment">The comment that will be deleted</param>
        private void DeleteComment(object comment)
        {
            _controller.RemoveComment(comment as Comment);
            Features.AddNotification(new Notification("Comment was removed", Notification.INFO));
            OnPropertyChanged(nameof(CommentList));
        }

        /// <summary>
        /// Starts the editing process for a comment
        /// </summary>
        /// <param name="comment">The comment that is being edited</param>
        private void EditComment(object comment)
        {
            _controller.EditComment(comment as Comment);
            OnPropertyChanged(nameof(CommentList));
        }

        /// <summary>
        /// Updates the content of a comment
        /// </summary>
        /// <param name="comment">The comment that is being updated</param>
        private void UpdateComment(object comment)
        {
            _controller.UpdateComment(comment as Comment);
            OnPropertyChanged(nameof(CommentList));
        }
    }
}
