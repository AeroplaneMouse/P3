using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AMS.Database.Repositories.Interfaces;
using AMS.Models;
using System.Linq;
using AMS.Interfaces;
using Type = System.Type;

namespace AMS.Helpers
{
    public class TagHelper
    {
        private Tag _parent;
        public bool CanApplyParentTags = false;
        private List<Tag> _tags;
        private List<User> _users;
        private bool _hasSuggestions;
        private readonly ITagRepository _tagRepository;
        private readonly IUserRepository _userRepository;
        private  List<ITagable> SuggestedTags;
        private ObservableCollection<ITagable> AppliedTags { get; set; }

        public TagHelper(ITagRepository tagRepository, IUserRepository userRepository)
        {
            _tagRepository = tagRepository;
            _userRepository = userRepository;

            AppliedTags = new ObservableCollection<ITagable>();
            SuggestedTags = new List<ITagable>();

            Reload();
            SetParent();
        }

        public void Reload()
        {
            _tags = (List<Tag>) _tagRepository.GetAll();
            _users = (List<User>) _userRepository.GetAll();
        }

        public List<ITagable> Suggest(string input)
        {
            List<ITagable> result = new List<ITagable>();

            if (IsParentSet())
            {
                if (_parent.TagId == 1)
                {
                    result.AddRange(_users.Where(u => u.Username.StartsWith(input, StringComparison.InvariantCultureIgnoreCase) 
                                                      && AppliedTags.Count(q => q.TagId == u.TagId) == 0 ));
                }else{
                    result.AddRange(SuggestedTags.Where(t => t.TagLabel.StartsWith(input, StringComparison.InvariantCultureIgnoreCase) 
                                                             && AppliedTags.Count(q => q.TagId == t.TagId) == 0 ));
                }
            }
            else
            {
                foreach (var item in SuggestedTags)
                {
                    if ((item.ChildrenCount > 0 || item.TagId == 1) 
                        && (!AppliedTags.Contains(item) || !ContainsAllChildrenOfParent(item)))
                        result.Add(item);
                    
                    else if (item.ChildrenCount == 0 && !AppliedTags.Contains(item))
                        result.Add(item);
                }
              
                if(result.Any() && !string.IsNullOrEmpty(input))
                    result = result.Where(t => t.TagLabel.Contains(input, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
            
            _hasSuggestions = result.Any();

            return result;
        }

        public bool HasSuggestions()
        {
            return _hasSuggestions;
        }

        public void AddTag(ITagable tag)
        {
            if (!AppliedTags.Contains(tag))
            {
                if (tag.ParentId == 0 && tag.ChildrenCount > 0)
                {
                    if (CanApplyParentTags)
                    {
                        AppliedTags.Add(tag);
                    }
                }
                else
                {
                    ApplyParentIfNeeded(tag);
                    AppliedTags.Add(tag);
                }
            }
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
            
            return tag.TagType == typeof(User) ? _tags.Single(u => u.ID == 1) : null;
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
                var count = AppliedTags.Count(t => t.ParentId == tag.ParentId && t.TagId != tag.TagId);

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
                    try
                    {
                        var parentUserTag = AppliedTags.SingleOrDefault(u => u.TagId == 1 && u.TagType == typeof(Tag));
                        AppliedTags.Remove(parentUserTag);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }

            return false;
        }

        private bool ContainsAllChildrenOfParent(ITagable tag)
        {
            if (tag.ParentId > 0)
                return false;
            
            if (tag.TagId == 1)
                return AppliedTags.Count(u => u.TagType == typeof(User)) == _users.Count; // Users

            return AppliedTags.Count(t => t.ParentId == tag.TagId) == tag.ChildrenCount;
        }

        public void SetParent(Tag tag=null)
        {
            SuggestedTags.Clear();
            SuggestedTags.AddRange(tag != null ? _tags.Where(a => a.ParentID == tag.ID).ToList() : _tags.Where(a => a.ParentID == 0).ToList());
            _parent = tag;
        }

        public bool AllParentChildrenTagged()
        {
            if (IsParentSet())
                return AppliedTags.Count(t => t.ParentId == _parent.ID) == _parent.ChildrenCount;

            return false;
        }

        public ObservableCollection<ITagable> GetAppliedTags(bool includeParents=false)
        {
            if (includeParents)
                return AppliedTags;
            
            return new ObservableCollection<ITagable>(AppliedTags.Where(t => t.ParentId > 0 || (t.ParentId == 0 && t.ChildrenCount == 0 && (t.TagId != 1 && t.TagType == typeof(Tag)))));
        }

        public List<ulong> GetAppliedTagIds(Type type)
        {
            return AppliedTags.Where(t => t.TagType == type).Select(t => t.TagId).ToList();
        }

        public bool IsParentSet() => _parent != null;

        public bool ContainsAllChildrenFromParent()
        {
            if (IsParentSet())
                return ContainsAllChildrenOfParent(_parent);

            return true;
        }

        public void SetCurrentTags(ObservableCollection<ITagable> tags)
        {
            foreach(ITagable tag in tags)
            {
                if (!AppliedTags.Contains(tag))
                {
                    ApplyParentIfNeeded(tag);
                    AddTag(tag);
                }
            }
        }

        public Tag GetParent() => _parent;
    }
}