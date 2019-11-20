using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AMS.Database.Repositories;
using AMS.Database.Repositories.Interfaces;
using AMS.Models;
using System.Linq;

namespace AMS.Helpers
{
    public class TagHelper
    {
        private Tag _parent;
        public List<ITagable> SuggestedTags;
        private List<Tag> _tags;
        private List<User> _users;
        private bool _hasSuggestions = false;
        public ObservableCollection<ITagable> AppliedTags { get; set; }
        private readonly ITagRepository _tagRepository;
        private readonly IUserRepository _userRepository;

        public TagHelper(ITagRepository tagRepository, IUserRepository userRepository, ObservableCollection<ITagable> tags=null)
        {
            _tagRepository = tagRepository;
            _userRepository = userRepository;

            AppliedTags = tags ?? new ObservableCollection<ITagable>();
            SuggestedTags = new List<ITagable>();

            Reload();
            Parent(null);
        }

        public void Reload()
        {
            _tags = (List<Tag>) _tagRepository.GetAll();
            _users = (List<User>) _userRepository.GetAll();
        }

        public List<ITagable> Suggest(string input)
        {
            List<ITagable> result = new List<ITagable>();
            
            if (_parent != null && _parent.ID == 1)
            {
                result.AddRange(_users.Where(u => u.Username.StartsWith(input, StringComparison.InvariantCultureIgnoreCase) 
                                                  && !AppliedTags.Contains(u)).ToList());
            }
            else
            {
                result.AddRange(SuggestedTags.Where(t => t.TagLabel.StartsWith(input, StringComparison.InvariantCultureIgnoreCase)
                                                  && (!AppliedTags.Contains(t))).ToList());
            }

            _hasSuggestions = result.Any();

            return result;
        }

        public bool HasSuggestions()
        {
            return _hasSuggestions;
        }

        public void AddToQuery(ITagable tag)
        { 
            AppliedTags.Add(tag);
        }

        public void RemoveFromQuery(ITagable tag)
        {
            AppliedTags.Remove(tag);
        }

        public void Parent(Tag tag=null)
        {
            SuggestedTags.AddRange(tag != null ? _tags.Where(a => a.ParentID == tag.ID).ToList() : _tags.Where(a => a.ParentID == 0).ToList());
            _parent = tag;
        }

        public bool IsParentSet() => _parent != null;

        public Tag GetParent() => _parent;

        public List<Tag> Tags()
        {
            return null;
        }
    }
}