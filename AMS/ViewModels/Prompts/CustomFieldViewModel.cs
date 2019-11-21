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
        private bool _isCustom;

        public override event PromptEventHandler PromptElapsed;

        public string Name { get; set; } = String.Empty;
        public string DefaultValue { get; set; } = String.Empty;
        public bool DefaultBool { get; set; } = false;
        public string SelectedDate { get; set; }
        public bool IsRequired { get; set; } = false;
        public Field.FieldType SelectedFieldType { get; set; }
        public List<Field.FieldType> FieldTypes { get; set; } = (List<Field.FieldType>)Field.GetTypes();


        public CustomFieldViewModel(string message, PromptEventHandler handler, bool isCustom = false)
            : base(message, handler)
        {
            _isCustom = isCustom;
            SelectedFieldType = Field.FieldType.TextBox;
        }


        protected override void Accept()
        {
            Console.WriteLine($"Default value: { DefaultValue }");
            if (SelectedFieldType == 0)
                return;

            if (SelectedFieldType == Field.FieldType.Checkbox)
                DefaultValue = DefaultBool ? "1" : "0";

            if (SelectedFieldType == Field.FieldType.Date)
                DefaultValue = SelectedDate;

            _newField = new Field(Name, "", SelectedFieldType, DefaultValue, IsRequired, _isCustom);
            PromptElapsed?.Invoke(this, new FieldInputPromptEventArgs(true, _newField));
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