using System;
using System.Collections.Generic;
using System.Linq;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Views;

namespace Asset_Management_System.Functions
{
    public class Tagging
    {
        private Tag _parent = null;
        private List<Tag> _suggestedTags;
        
        private List<Tag> _tags;
        private List<User> _users;
        
        private List<ITagable> _tagged;
        private TagRepository _tagRep;
        private UserRepository _userRep;

        public Tagging(List<ITagable> tags=null)
        {
            _tagRep = new TagRepository();
            _userRep = new UserRepository();
            
            _tagged = tags ?? new List<ITagable>();
            _parent = _tagRep.GetById(1);
            
            Reload();
        }

        public void Reload()
        {
            _tags = (List<Tag>) _tagRep.GetAll();
            _users = (List<User>) _userRep.GetAll();
        }

        public List<ITagable> Suggest(string input)
        {
            List<ITagable> result = new List<ITagable>();
            
            if (_parent != null && _parent.ID == 1)
            {
                result.AddRange(_users.Where(u => u.Username.StartsWith(input, StringComparison.InvariantCultureIgnoreCase) 
                                         && !_tagged.Contains(u)).ToList());
            }
            else
            {
                result.AddRange(_suggestedTags.Where(t => t.Name.StartsWith(input, StringComparison.InvariantCultureIgnoreCase)
                                         && !_tagged.Contains(t)).ToList());
            }

            return result;
        }

        public void AddToQuery(ITagable tag)
        { 
            _tagged.Add(tag);
        }

        public void RemoveFromQuery(ITagable tag)
        {
            _tagged.Remove(tag);
        }

        public void Parent(Tag tag=null)
        {
            _suggestedTags = tag != null ? _tags.Where(a => a.ParentID == tag.ID).ToList() : _tags.Where(a => a.ParentID == 0).ToList();
            _parent = tag;
        }

        public List<Tag> Tags()
        {
            return null;
        }
    }
}