using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AMS.Database.Repositories.Interfaces;
using AMS.Models;
using System.Linq;
using AMS.Interfaces;
using Type = System.Type;
using AMS.ViewModels;

namespace AMS.Helpers
{
    public class TagHelper
    {
        private Logging.Logger _logger = new Logging.Logger(Features.LogRepository);
        private Tag _parent;
        public bool CanApplyParentTags = false;
        private List<Tag> _tags;
        private List<User> _users;
        private bool _hasSuggestions;
        private readonly ITagRepository _tagRepository;
        private readonly IUserRepository _userRepository;
        private List<ITagable> SuggestedTags;
        private List<ITagable> EffectedTags; // List of tags that was effected by add or removing a tag.
        private ObservableCollection<ITagable> AppliedTags { get; set; }

        public TagHelper(ITagRepository tagRepository, IUserRepository userRepository)
        {
            _tagRepository = tagRepository;
            _userRepository = userRepository;

            AppliedTags = new ObservableCollection<ITagable>();
            SuggestedTags = new List<ITagable>();
            EffectedTags = new List<ITagable>();

            Reload();
            SetParent();
        }

        /// <summary>
        /// Reloads the tag and user lists
        /// </summary>
        public void Reload()
        {
            _tags = _tagRepository.GetAll().ToList();
            _users = _userRepository.GetAll().ToList();
        }

        private List<ITagable> GetTagListWithoutDuplicates(List<ITagable> tags)
        {
            List<ITagable> cleanTagList = tags.GroupBy(t => t.TagLabel)
                            .Select(x => x.FirstOrDefault())
                            .ToList();

            return cleanTagList;
        }

        /// <summary>
        /// Finds and returns the ITagables that match the search query
        /// </summary>
        /// <param name="input"></param>
        /// <returns>List of suggestions for the search query</returns>
        public List<ITagable> Suggest(string input)
        {
            List<ITagable> result = new List<ITagable>();

            if (IsParentSet())
            {
                if (_parent.TagId == 1)
                {
                    result.AddRange(_users.Where(u => u.Username.StartsWith(input, StringComparison.InvariantCultureIgnoreCase) 
                                                      && !AppliedTags.Contains(u)));
                }else{
                    result.AddRange(SuggestedTags.Where(t => t.TagLabel.StartsWith(input, StringComparison.InvariantCultureIgnoreCase) 
                                                      && !AppliedTags.Contains(t)));
                }
            }
            else
            {
                foreach (var item in SuggestedTags)
                {
                    if ((item.NumberOfChildren > 0 || item.TagId == 1) 
                        && (!AppliedTags.Contains(item) || !ContainsAllChildrenOfParent(item)))
                    {
                        result.Add(item);
                    }
                    
                    else if (item.NumberOfChildren == 0 && !AppliedTags.Contains(item))
                        result.Add(item);
                }
              
                if(result.Any() && !string.IsNullOrEmpty(input))
                    result = result.Where(t => t.TagLabel.Contains(input, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
            
            _hasSuggestions = result.Any();

            return result;
        }

        /// <summary>
        /// Returns whether there are any suggestions for the search query
        /// </summary>
        /// <returns></returns>
        public bool HasSuggestions()
        {
            return _hasSuggestions;
        }

        /// <summary>
        /// Adds a tag to the AppliedTags list, and the list of effected Tags.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public List<ITagable> AddTag(ITagable tag)
        {
            EffectedTags.Clear();
            
            if (!AppliedTags.Contains(tag))
            {
                if (tag.ParentId == 0 && tag.NumberOfChildren > 0)
                {
                    if (CanApplyParentTags)
                    {
                        foreach(ITagable tagInAllTagsList in _tags.Where(t => t.TagLabel == tag.TagLabel))
                        {
                            AppliedTags.Add(tagInAllTagsList);
                        }
                    }
                }
                else
                {
                    if (tag is User)
                    {
                        AppliedTags.Add(tag);
                    }
                    else
                    {
                        foreach (ITagable tagInAllTagsList in _tags.Where(t => t.TagLabel == tag.TagLabel))
                        {
                            AppliedTags.Add(tagInAllTagsList);
                        }
                    }
                    ApplyParentIfNeeded(tag);
                }
                
                EffectedTags.Add(tag);
            }
            
            return EffectedTags;
        }

        /// <summary>
        /// Removes a tag from the AppliedTags list, and returns the list of effected Tags.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public List<ITagable> RemoveTag(ITagable tag)
        {
            EffectedTags.Clear();
            if (tag is User)
            {
                RemoveParentIfNeeded(tag);
                AppliedTags.Remove(tag);
                EffectedTags.Add(tag);
            }
            else
            {
                List<ITagable> currentlyAppliedTags = new List<ITagable>(AppliedTags.Where(t => t.FullTagLabel == tag.FullTagLabel));
                foreach (ITagable appliedTag in currentlyAppliedTags)
                {
                    RemoveParentIfNeeded(appliedTag);
                    AppliedTags.Remove(appliedTag);
                    EffectedTags.Add(appliedTag);
                }
            }
            return EffectedTags;
        }

        /// <summary>
        /// Finds and returns the parentTag of the given ITagable
        /// </summary>
        /// <param name="tag"></param>
        /// <returns>Parent tag of given ITagble</returns>
        private Tag GetTagParent(ITagable tag)
        {
            if (tag.TagType == typeof(Tag))
            {
                var item = (Tag) tag;

                if (item.ParentId > 0)
                {
                    return _tags.Single(t => t.ID == item.ParentId);
                }
            }
            
            return tag.TagType == typeof(User) ? _tags.Single(u => u.ID == 1) : null;
        }

        /// <summary>
        /// Determines if the parentTag of the given ITagable should be applied.
        /// If so adds the parentTag to the AppliedTags list
        /// and the EffectedTags list.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns>Whether the parentTag was applied</returns>
        private bool ApplyParentIfNeeded(ITagable tag)
        {
            if (CanApplyParentTags || (tag.ParentId == 0 && tag.NumberOfChildren > 0))
                return false;
            
            List<Tag> parents = new List<Tag>();
            foreach (Tag tagWithParentLabel in _tags.Where(t => t.ParentId == 0 && (_parent != null ? t.TagLabel == _parent.TagLabel : true)))
            {
                parents.Add(tagWithParentLabel);
            }

            if (parents == null)
                return false;

            foreach (Tag parentTag in parents)
            {
                if (!AppliedTags.Contains(parentTag))
                {
                    AppliedTags.Add(parentTag);
                    EffectedTags.Add(parentTag);
                }
            }
            return true;
        }

        /// <summary>
        /// Determines if the parentTag should be removed.
        /// If so removes it from the AppliedTags list,
        /// and adds it the EffectedTags list
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
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
                        EffectedTags.Add(parent);
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
                        EffectedTags.Add(parentUserTag);
                        AppliedTags.Remove(parentUserTag);
                        return true;
                    }
                    catch (Exception e)
                    {
                        _logger.AddEntry(e);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines and returns if the AppliedTags list contain all the child Tags of the given ITagable
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        private bool ContainsAllChildrenOfParent(ITagable tag)
        {
            if (tag.ParentId > 0)
                return false;
            
            if (tag.TagId == 1)
                return AppliedTags.Count(u => u.TagType == typeof(User)) == _users.Count; // Users

            return AppliedTags.Count(t => t.ParentId == tag.TagId) == tag.NumberOfChildren;
        }

        /// <summary>
        /// Determines and returns if all the children of the current parentTag are in the AppliedTags list
        /// </summary>
        /// <returns></returns>
        public bool AllChildrenOfCurrentParentAttached()
        {
            if (IsParentSet())
                return AppliedTags.Count(t => t.ParentId == _parent.ID) == _parent.NumberOfChildren;

            return false;
        }
        
        /// <summary>
        /// Determines and returns if all the children of the current parentTag are in the AppliedTags list
        /// </summary>
        /// <returns></returns>
        public bool ContainsAllChildrenFromCurrentParent()
        {
            if (IsParentSet())
                return ContainsAllChildrenOfParent(_parent);

            return true;
        }

        /// <summary>
        /// Returns the AppliedTags list.
        /// If parameter includeParents is true, also adds the Tags with child Tags to the list.
        /// </summary>
        /// <param name="includeParents"></param>
        /// <returns></returns>
        public ObservableCollection<ITagable> GetAppliedTags(bool includeParents=false)
        {
            if (includeParents)
                return AppliedTags;
            
            return new ObservableCollection<ITagable>(AppliedTags.Where(t => t.ParentId > 0 || (t.ParentId == 0 && t.NumberOfChildren == 0 && (t.TagId != 1 && t.TagType == typeof(Tag)))));
        }
        
        /// <summary>
        /// Assigns the given list of ITagable to the AppliedTags list
        /// </summary>
        /// <param name="tags"></param>
        public void SetAppliedTags(ObservableCollection<ITagable> tags)
        {
            AppliedTags = tags;
        }
        
        /// <summary>
        /// Returns a list of the ID's of the ITagables in the AppliedTags list
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<ulong> GetAppliedTagIds(Type type)
        {
            return AppliedTags.Where(t => t.TagType == type).Select(t => t.TagId).ToList();
        }

        /// <summary>
        /// Returns the currently set ParentTag
        /// </summary>
        /// <returns></returns>
        public Tag GetParent() => _parent;
        
        /// <summary>
        /// Assigns the given Tag to the current parentTag.
        /// If a tag is given adds all children of this tag to the SuggestedTags list.
        /// If no tag is given, adds all the tags with no parentTag to the SuggestedTags list.
        /// </summary>
        /// <param name="tag"></param>
        public void SetParent(Tag tag=null)
        {
            SuggestedTags.Clear();
            SuggestedTags.AddRange(tag != null ? _tags.Where(t => t.ParentId == tag.ID || (t.FullTagLabel.StartsWith(tag.TagLabel) && t.FullTagLabel.Contains(char.ConvertFromUtf32(0x1f852)))).ToList() : _tags.Where(t => t.ParentId == 0).ToList());

            SuggestedTags = GetTagListWithoutDuplicates(SuggestedTags);

            _parent = tag;
        }
        
        /// <summary>
        /// Returns whether the current parentTag is set.
        /// </summary>
        /// <returns></returns>
        public bool IsParentSet() => _parent != null;
    }
}