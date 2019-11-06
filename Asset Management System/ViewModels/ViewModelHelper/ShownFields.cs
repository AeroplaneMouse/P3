using System;
using System.Collections.ObjectModel;
using Asset_Management_System.Models;

namespace Asset_Management_System.ViewModels.ViewModelHelper
{
    public class ShownField
    {
        public string Name { get; set; }
        public Field Field { get; set; }
        public ObservableCollection<Tag> FieldTags { get; set; }
        
        public string RequiredNotation { get => "*"; }

        public ShownField(Field field)
        {
            FieldTags = new ObservableCollection<Tag>();

            Name = field.Hash;
            this.Field = field;
        }

        /// <summary>
        /// If a field and a shown field are the same, it returns true.
        /// </summary>
        /// <returns></returns>
        public bool ShownFieldToFieldComparator(Field field)
        {
            return field.Equals(Field);
        }
    }
}