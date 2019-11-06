﻿using Asset_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Asset_Management_System.Views
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
            Field field = item as Field;
            switch (field?.Type)
            {
                case Field.FieldType.Textarea: // Textbox
                    return element?.FindResource("TextBoxFieldStyle") as DataTemplate;
                case Field.FieldType.Textbox: // String
                    return element?.FindResource("StringFieldStyle") as DataTemplate;
                case Field.FieldType.Integer: // Integer
                    return element?.FindResource("IntegerFieldStyle") as DataTemplate;
                case Field.FieldType.Date: // Date
                    return element?.FindResource("DateFieldStyle") as DataTemplate;
                case Field.FieldType.Boolean: // Boolean
                    return element?.FindResource("BooleanFieldStyle") as DataTemplate;
                default:
                    throw new NotSupportedException("Wrong formatting syntax.");
            }
        }
    }
}
