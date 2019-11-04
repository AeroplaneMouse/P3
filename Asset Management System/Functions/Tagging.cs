using System.Collections.Generic;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Views;
using Asset_Management_System.Models;

namespace Asset_Management_System.Functions
{
    public class Tagging
    {
        private List<Tag> _tags;
        private List<Tag> _tagged;
        
        public Tagging()
        {
            Reload();
        }

        public void Reload()
        {
            TagRepository rep = new TagRepository();
            _tags = (List<Tag>) rep.GetAll();
        }

        public List<Tag> Suggest(string input)
        {
            return null;
        }

        public void Attach(int id)
        {
            
        }

        public List<Tag> Tags()
        {
            return _tagged;
        }
    }
}