using System;
using System.Windows;
using System.Windows.Controls;

namespace Asset_Management_System.ViewModels.Commands.ViewModelHelper
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
            ShowFields field = item as ShowFields;
            switch (field?.Field.FieldType)
            {
                case 1: // Textbox
                    return element?.FindResource("TextBoxFieldStyle") as DataTemplate;
                case 2: // String
                    return element?.FindResource("StringFieldStyle") as DataTemplate;
                case 3: // Integer
                    return element?.FindResource("IntegerFieldStyle") as DataTemplate;
                case 4: // Date
                    return element?.FindResource("DateFieldStyle") as DataTemplate;
                case 5: // Boolean
                    return element?.FindResource("BooleanFieldStyle") as DataTemplate;
                default:
                    throw new NotSupportedException("Wrong formatting syntax.");
            }
        }
    }
}