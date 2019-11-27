using System;
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

        public TagHelper(ITagRepository tagRepository, IUserRepository userRepository)
        {
            _tagRepository = tagRepository;
            _userRepository = userRepository;

            AppliedTags = new ObservableCollection<ITagable>();
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

            if (IsParentSet())
            {
                if (_parent.TagId == 1)
                {
                    result.AddRange(_users.Where(u => u.Username.StartsWith(input, StringComparison.InvariantCultureIgnoreCase) 
                                                      && !AppliedTags.Contains(u)).ToList());
                }else{
                    result.AddRange(SuggestedTags.Where(t => t.TagLabel.StartsWith(input, StringComparison.InvariantCultureIgnoreCase) 
                                                             && !AppliedTags.Contains(t)).ToList());
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

                result = result.Where(t => t.TagLabel.Contains(input, StringComparison.InvariantCultureIgnoreCase)).ToList();


                //result.AddRange(SuggestedTags
                //    .Where(t => t.ChildrenCount > 0)
                //    .Where(t => t.TagId != 1 && t.ChildrenCount == 0 && !AppliedTags.Contains(t))
                //    .Where(t => (t.TagId == 1 || t.ChildrenCount > 0) && (!AppliedTags.Contains(t) || !ContainsAllChildrenOfParent(t)))
                //    .Where(t => t.TagLabel.Contains(input, StringComparison.InvariantCultureIgnoreCase))
                //    .ToList());
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
                    try
                    {
                        var parentUserTag = AppliedTags.Single(u => u.TagId == 1 && u.TagType == typeof(Tag));
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
            {
                // Users
                return AppliedTags.Count(u => u.TagType == typeof(User)) == _users.Count;
            }

            return AppliedTags.Count(t => t.ParentId == tag.TagId) == tag.ChildrenCount;
        }

        public void Parent(Tag tag=null)
        {
            SuggestedTags.Clear();
            SuggestedTags.AddRange(tag != null ? _tags.Where(a => a.ParentID == tag.ID).ToList() : _tags.Where(a => a.ParentID == 0).ToList());
            _parent = tag;
        }

        public bool AllParentChildrenTagged()
        {
            if (IsParentSet())
            {
                return AppliedTags.Count(t => t.ParentId == _parent.ID) == _parent.ChildrenCount;
            }

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
            AppliedTags = tags;
        }

        public Tag GetParent() => _parent;
    }
}