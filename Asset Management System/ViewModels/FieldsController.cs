﻿using Asset_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Asset_Management_System.ViewModels.Interfaces;
using Asset_Management_System.ViewModels.ViewModelHelper;

namespace Asset_Management_System.ViewModels
{
    public class FieldsController : IFieldManager
    {
        
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
        
        private void LoadFields(Asset asset,ObservableCollection<ShownField> HiddenFields,ObservableCollection<ShownField> FieldsList)
        {
            foreach (var field in asset.FieldsList)
            {
                if (field.IsHidden)
                    HiddenFields.Add(new ShownField(field));
                else
                    FieldsList.Add(new ShownField(field));
            }
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