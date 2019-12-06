using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Services.Interfaces;

namespace Asset_Management_System.Services
{
    public class CommentService : ICommentService
    {
        private ICommentRepository _rep;

        public CommentService(ICommentRepository rep)
        {
            _rep = rep;
        }

        public IRepository<Comment> GetRepository() => _rep;

        public string GetName(Comment comment) => comment.GetId().ToString();
    }
}