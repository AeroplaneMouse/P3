using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Services.Interfaces;
using Asset_Management_System.ViewModels.ViewModelHelper;

namespace Asset_Management_System.ViewModels.Controllers
{
    public class TagController
    {
        private ITagService _service;
        private ITagRepository _rep;
        private string _color;
        
        public Tag Tag { get; set; }
        
        public List<Tag> ParentTagsList
        {
            get
            {
                List<Tag> parentTagsList = new List<Tag>()
                {
                    new Tag()
                    {
                        Name = "[No Parent Tag]",
                        ParentID = 0,
                        Color = _color
                    }
                };
                foreach (Tag parentTag in (List<Tag>) _rep.GetParentTags())
                    parentTagsList.Add(parentTag);

                return parentTagsList;
            }
        }
        
        public int SelectedParentIndex { get; set; }

        
        public TagController(Tag inputTag, ITagService service)
        {
            //Tag = tag;
            _service = service;
            _rep = (ITagRepository) _service.GetSearchableRepository();
            _color = CreateRandomColor();
            
            //FieldsList = new ObservableCollection<ShownField>();
            
            if (inputTag != null)
            {
                Tag = inputTag;
                //Editing = true;
                LoadFields();
            }
            else
            {
                Tag = new Tag();
                //Editing = false;
            }
        }

        /// <summary>
        /// Runs through the saved fields within the tag, and adds these to the fieldList.
        /// </summary>
        /// <returns></returns>
        protected void LoadFields()
        {
            foreach (Field field in Tag.FieldsList)
            {
                if (field.IsHidden) {}
                    //HiddenFields.Add(new ShownField(field));
                //else
                    //FieldsList.Add(new ShownField(field));
            }

           //ConnectTags();

            //Set the selected parent to the parent of the chosen tag
            int i = ParentTagsList.Count;
            while (i > 0 && ParentTagsList[i - 1].ID != Tag.ParentID)
                i--;

            if (i > 0)
                SelectedParentIndex = i - 1;
            
            //OnPropertyChanged(nameof(SelectedParentIndex));
        }
        
        private string CreateRandomColor()
        {
            //Creates an instance of the Random, to create pseudo random numbers
            Random random = new Random();

            //Creates a hex values from three random ints converted to bytes and then to string
            string hex = "#" + ((byte) random.Next(25, 230)).ToString("X2") +
                         ((byte) random.Next(25, 230)).ToString("X2") + ((byte) random.Next(25, 230)).ToString("X2");

            return hex;
        }
        
    }
}