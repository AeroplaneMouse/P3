using Asset_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Asset_Management_System.ViewModels.Interfaces;
using Asset_Management_System.ViewModels.ViewModelHelper;

namespace Asset_Management_System.ViewModels
{
    public abstract class FieldsController : Base.BaseViewModel, IFieldManager
    {
        protected abstract void LoadFields();
        public ObservableCollection<ShownField> FieldsList { get; set; } = new ObservableCollection<ShownField>();
        protected bool Editing;


        public ICommand AddFieldCommand { get; set; }
        public static ICommand RemoveFieldCommand { get; set; }

        public List<string> PromptManager(string label, out bool required)
        {
            var dialog = new Views.PromptForFields(label);
            List<string> outputList = new List<string>();
            required = false;
            if (dialog.ShowDialog() == true)
            {
                if (dialog.DialogResult == true)
                {
                    string name = dialog.FieldName;
                    string defaultValue = dialog.DefaultValueText;
                    outputList.Add(name);
                    outputList.Add(defaultValue);
                    required = dialog.Required;
                }
            }

            return outputList;
        }

        public bool AddField(DoesContainFields obj, Field field)
        {
            throw new NotImplementedException();
        }

        public bool UpdateField(DoesContainFields obj, Field field)
        {
            throw new NotImplementedException();
        }

        public bool RemoveField(DoesContainFields obj, Field field)
        {
            throw new NotImplementedException();
        }
    }
}