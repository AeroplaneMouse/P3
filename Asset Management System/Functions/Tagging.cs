using System.Collections.Generic;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Views;
using Asset_Management_System.Models;

namespace Asset_Management_System.Functions
{
    public class Tagging
    {
        private Tag _parent = null;
        private List<Tag> _tags;
        private List<Tag> _tagged;
        private TagRepository _tagRep;
        private UserRepository _userRep;

        public Tagging()
        {
            _tagRep = new TagRepository();
            Reload();
        }

        public void Reload()
        {
            _tags = (List<Tag>) _tagRep.GetAll();
        }

        public List<Tag> Suggest(string input)
        {
            if (_parent != null)
            {
                
            }
            else
            {
                
            }

            return null;
        }

        public void Attach(Tag tag)
        {
            
        }

        public void Detach(Tag tag)
        {
            
        }

        public void Parent(Tag tag=null)
        {
            _parent = tag;
        }

        public List<Tag> Tags()
        {
            return _tagged;
        }
    }
}