using AMS.Controllers.Interfaces;
using AMS.Database.Repositories.Interfaces;
using AMS.Models;

namespace AMS.Controllers
{
    public class TagController : FieldController, ITagController
    {
        public TagController(Tag tag) : base(tag) => this.tag = tag;

        public Tag tag { get; set; }
        public ITagRepository tagRepository { get; set; }
        public ulong tagID;

        public void Save()
        {
            tagRepository.Insert(tag, out tagID);
        }

        public void Remove()
        {
            tagRepository.Delete(tag);
        }

        public void Update()
        {
            tagRepository.Update(tag);
        }
    }
}
