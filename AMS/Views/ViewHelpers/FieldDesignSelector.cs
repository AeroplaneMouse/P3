using System;
using System.Windows;
using System.Windows.Controls;
using AMS.Models;

namespace AMS.Views.ViewHelpers
{
    /// <summary>
    /// Class used for selecting a template for the fields.
    /// </summary>
    public class FieldDesignSelector : DataTemplateSelector
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
            Field currentField = item as Field;
            {
                if (currentField != null)
                    switch (currentField.Type)
                    {
                        case Field.FieldType.Textarea: // Textbox
                            return element?.FindResource("TextBoxFieldStyle") as DataTemplate;
                        case Field.FieldType.TextBox: // String
                            return element?.FindResource("StringFieldStyle") as DataTemplate;
                        case Field.FieldType.NumberField: // Integer
                            return element?.FindResource("IntegerFieldStyle") as DataTemplate;
                        case Field.FieldType.Date: // Date
                            return element?.FindResource("DateFieldStyle") as DataTemplate;
                        case Field.FieldType.Checkbox: // Boolean
                            return element?.FindResource("BooleanFieldStyle") as DataTemplate;
                        default:
                            throw new NotSupportedException("Wrong formatting syntax.");
                    }
            }
            return new DataTemplate();
        }
    }
}