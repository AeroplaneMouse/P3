﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AMS.Database.Repositories.Interfaces;
using AMS.Models;
using System.Linq;
using AMS.Interfaces;

namespace AMS.Helpers
{
    public class TagHelper
    {
        private Tag _parent;
        public bool CanApplyParentTags = false;
        private List<Tag> _tags;
        private List<User> _users;
        private bool _hasSuggestions = false;
        private readonly ITagRepository _tagRepository;
        private readonly IUserRepository _userRepository;
        private List<ITagable> SuggestedTags;
        private ObservableCollection<ITagable> AppliedTags { get; set; }

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

        public void ApplyTag(ITagable tag)
        {
            ApplyParentIfNeeded(tag);
            AppliedTags.Add(tag);
        }

        public void RemoveTag(ITagable tag)
        {
            RemoveParentIfNeeded(tag);
            AppliedTags.Remove(tag);
        }

        private Tag GetTagParent(ITagable tag)
        {
            if (tag.TagType == typeof(Tag))
            {
                var item = (Tag) tag;

                if (item.ParentID > 0)
                {
                    return _tags.Single(t => t.ID == item.ParentID);
                }
            }
            
            if(tag.TagType == typeof(User)){
                // return the users tag if it is a user.
                return _tags.Single(u => u.ID == 1);
            }

            return null;
        }

        private bool ApplyParentIfNeeded(ITagable tag)
        {
            if (CanApplyParentTags || (tag.ParentId == 0 && tag.ChildrenCount > 0))
                return false;
            
            var parent = GetTagParent(tag);

            if (parent == null || AppliedTags.Contains(parent))
                return false;
            
            AppliedTags.Add(parent);
            return true;
        }

        private bool RemoveParentIfNeeded(ITagable tag)
        {
            if (CanApplyParentTags || tag.ParentId == 0)
                return false;
            
            if (tag.TagType == typeof(Tag)){
                int count = AppliedTags.Count(t => t.ParentId == tag.ParentId && t.TagId != tag.TagId);

                if (count == 0)
                {
                    var parent = GetTagParent(tag);

                    if(parent != null && AppliedTags.Contains(parent)){
                        AppliedTags.Remove(parent);
                        return true;
                    }

                    return false;
                }
            }
            
            if (tag.TagType == typeof(User)){
                // return the users tag if it is a user.
                int count = AppliedTags.Count(u => u.TagType == typeof(User));

                if (count <= 1)
                {
                    AppliedTags.Remove(AppliedTags.Single(u => u.TagId == 1 && u.TagType == typeof(User)));
                        return true;
                }
            }

            return false;
        }

        public void Parent(Tag tag=null)
        {
            SuggestedTags.Clear();
            SuggestedTags.AddRange(tag != null ? _tags.Where(a => a.ParentID == tag.ID).ToList() : _tags.Where(a => a.ParentID == 0).ToList());
            _parent = tag;
        }

        public ObservableCollection<ITagable> GetAppliedTags(bool includeParents=false)
        {
            if (includeParents)
                return AppliedTags;
            
            return new ObservableCollection<ITagable>(AppliedTags.Where(t => t.ParentId > 0 || (t.ParentId == 0 && t.ChildrenCount == 0)));
        }

        public List<ulong> GetAppliedTagIds(Type type)
        {
            return AppliedTags.Where(t => t.TagType == type).Select(t => t.TagId).ToList();
        }

        public bool IsParentSet() => _parent != null;

        public Tag GetParent() => _parent;
    }
}