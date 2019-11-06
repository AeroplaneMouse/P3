using Asset_Management_System.Events;
using Asset_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Asset_Management_System.ViewModels.Prompts
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
        }


        protected override void Accept()
        {
            if (SelectedFieldType == Field.FieldType.Boolean)
                DefaultValue = DefaultBool ? "1" : "0";

            if (SelectedFieldType == Field.FieldType.Date)
                DefaultValue = SelectedDate;

            _newField = new Field(Name, "", SelectedFieldType, DefaultValue, IsRequired, _isCustom);
            PromptElapsed?.Invoke(this, new PromptEventArgs(true, _newField));
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
                    Field.FieldType.Textbox => element?.FindResource("Box") as DataTemplate,
                    Field.FieldType.Integer => element?.FindResource("Box") as DataTemplate,
                    Field.FieldType.Date => element?.FindResource("Date") as DataTemplate,
                    Field.FieldType.Boolean => element?.FindResource("Boolean") as DataTemplate,
                    _ => element?.FindResource("Box") as DataTemplate
                };
            }
            else
                throw new NotSupportedException("Wrong formatting syntax.");

            //Field.FieldType type = item as Field.FieldType;
            //return element?.FindResource()



                ////ShownField field = item as ShownField;
                //switch (item as Field.FieldType)
                //{
                //    case Field.FieldType.Textarea: // Textbox
                //        return element?.FindResource("Area") as DataTemplate;
                //    case Field.FieldType.Textbox: // String
                //        return element?.FindResource("Box") as DataTemplate;
                //    case Field.FieldType.Integer: // Integer
                //        return element?.FindResource("Box") as DataTemplate;
                //    case Field.FieldType.Date: // Date
                //        return element?.FindResource("Box") as DataTemplate;
                //    case Field.FieldType.Boolean: // Boolean
                //        return element?.FindResource("Box") as DataTemplate;
                //    default:
                //        throw new NotSupportedException("Wrong formatting syntax.");
                //}
        }
    }
}