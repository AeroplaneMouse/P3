﻿using AMS.Controllers.Interfaces;
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
        public string NewComment { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand UpdateCommand{ get; set; }
        public ObservableCollection<Comment> CommentList => new ObservableCollection<Comment>(_controller.CommentList);
        private ICommentListController _controller { get; set; }

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

        private void EditComment(object comment)
        {
            _controller.EditComment(comment as Comment);
            OnPropertyChanged(nameof(CommentList));
        }

        private void UpdateComment(object comment)
        {
            _controller.UpdateComment(comment as Comment);
            OnPropertyChanged(nameof(CommentList));
        }
    }
}
