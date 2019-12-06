using System;
using AMS.Events;
using AMS.Models;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace AMS.ViewModels.Prompts
{
    public class CustomFieldViewModel : PromptViewModel
    {
        private Field _newField;
        private Field _oldField;
        private bool _isCustom;
        private bool _defaultBool = false;

        public override event PromptEventHandler PromptElapsed;

        public bool IsFieldNameNotNull => !string.IsNullOrEmpty(Name);
        public string Name { get; set; } = String.Empty;
        public string DefaultValue { get; set; } = String.Empty;
        public bool DefaultBool
        {
            get => _defaultBool;
            set
            {
                _defaultBool = value;
                DefaultValue = value.ToString();
            }
        }

        public string SelectedDate { get; set; } = "Current Date";
        public bool IsRequired { get; set; } = false;
        public Field.FieldType SelectedFieldType { get; set; }
        public List<Field.FieldType> FieldTypes { get; set; } = (List<Field.FieldType>)Field.GetTypes();
        public string PromptHeaderAndAcceptButtonText { get; set; }


        public CustomFieldViewModel(string message, PromptEventHandler handler, bool isCustom = false, Field inputField = null)
            : base(message, handler)
        {
            _isCustom = isCustom;
            _oldField = inputField;
            if (_oldField != null)
            {
                PromptHeaderAndAcceptButtonText = "Edit field";
                Name = inputField.Label;
                SelectedFieldType = inputField.Type;
                IsRequired = inputField.Required;

                if (SelectedFieldType == Field.FieldType.Checkbox)
                    DefaultBool = inputField.Content == "True";
                else if (SelectedFieldType == Field.FieldType.Date)
                    SelectedDate = inputField.Content;
                else
                    DefaultValue = inputField.Content;
            }
            else
            {
                PromptHeaderAndAcceptButtonText = "Add field";
                SelectedFieldType = Field.FieldType.TextBox;
            }
        }

        protected override void Accept()
        {
            if (SelectedFieldType == 0)
                return;

            if (SelectedFieldType == Field.FieldType.Date)
                DefaultValue = SelectedDate.Trim();

            _newField = new Field(Name, DefaultValue, SelectedFieldType, IsRequired, _isCustom);

            if (_oldField == null)
                PromptElapsed?.Invoke(this, new FieldInputPromptEventArgs(true, _newField));

            PromptElapsed?.Invoke(this, new FieldEditPromptEventArgs(true, _oldField, _newField));
        }

        protected override void Cancel()
        {
            PromptElapsed?.Invoke(this, new PromptEventArgs(false));
        }
    }
    public class DefaultValueDesignSelector : DataTemplateSelector
    {
        /// <summary>
        /// Function used for returning the correct template for the individual fields.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (item is Field.FieldType type)
            {
                return type switch
                {
                    Field.FieldType.Textarea => element?.FindResource("Area") as DataTemplate,
                    Field.FieldType.TextBox => element?.FindResource("Box") as DataTemplate,
                    Field.FieldType.NumberField => element?.FindResource("NumberField") as DataTemplate,
                    Field.FieldType.Date => element?.FindResource("Date") as DataTemplate,
                    Field.FieldType.Checkbox => element?.FindResource("Boolean") as DataTemplate,
                    _ => element?.FindResource("Box") as DataTemplate
                };
            }
            else
                throw new NotSupportedException("Wrong formatting syntax.");
        }
    }
}