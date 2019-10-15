using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Asset_Management_System.Models;

namespace Asset_Management_System.Views
{
    public abstract class FieldsController : Page
    {
        private int id = 0;
        public ObservableCollection<Field> FieldsList { get; set; }

        protected bool _editing;

        /// <summary>
        /// Function to add fields to the list of fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotSupportedException"></exception>
        protected void OnAddField(object sender, RoutedEventArgs e)
        {
            List<string> promptResults;
            switch ((sender as Button)?.Name)
            {
                case "AddTextField":
                    Console.WriteLine("Textfield added");
                    if ((promptResults = PromptManager("Text box", out bool required)).Count > 0)
                    {
                        FieldsList.Add(new Field(id++, promptResults[0], promptResults[1], 1, promptResults[1],
                            required));
                    }
                    break;
                case "AddStringField":
                    Console.WriteLine("StringField added");
                    if ((promptResults = PromptManager("String Field", out required)).Count > 0)
                    {
                        FieldsList.Add(new Field(id++, promptResults[0], promptResults[1], 1, promptResults[1],
                            required));
                    }
                    break;
                case "AddIntegerField":
                    Console.WriteLine("IntegerField added");
                    if ((promptResults = PromptManager("Integer FIeld", out required)).Count > 0)
                    {
                        FieldsList.Add(new Field(id++, promptResults[0], promptResults[1], 1, promptResults[1],
                            required));
                    }
                    break;
                case "AddDateField":
                    Console.WriteLine("Date Field added");
                    if ((promptResults = PromptManager("Date Field", out required)).Count > 0)
                    {
                        FieldsList.Add(new Field(id++, promptResults[0], promptResults[1], 1, promptResults[1],
                            required));
                    }
                    break;
                case "AddBooleanField":
                    Console.WriteLine("BooleanField added");
                    if ((promptResults = PromptManager("Boolean Field", out required)).Count > 0)
                    {
                        FieldsList.Add(new Field(id++, promptResults[0], promptResults[1], 1, promptResults[1],
                            required));
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }

            Console.WriteLine("List of the current fields after adding the field:");
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("ID  |   Field name   |   Content of the field");
            foreach (var test in FieldsList)
            {
                Console.WriteLine(test.ID + " | " + test.Label + " | " + test.Content);
            }

            Console.WriteLine("---------------------------------------");
        }

        /// <summary>
        /// Function to remove a field from the list of fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotSupportedException"></exception>
        protected void OnDeleteField(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button)?.Name)
            {
                case "DeleteTextField":
                    Console.WriteLine("Textfield removed");
                    break;
                case "DeleteStringField":
                    Console.WriteLine("StringField removed");
                    break;
                case "DeleteIntegerField":
                    Console.WriteLine("IntegerField removed");
                    break;
                case "DeleteDateField":
                    Console.WriteLine("DataField removed");
                    break;
                case "DeleteBooleanField":
                    Console.WriteLine("BooleanField removed");
                    break;
                default:
                    throw new NotSupportedException();
            }

            FieldsList.Remove((sender as FrameworkElement).DataContext as Field);
            Console.WriteLine("List of the current fields after removing the field:");
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("ID |Field name | Content of the field");
            foreach (var test in FieldsList)
            {
                Console.WriteLine(test.ID + " | " + test.Label + "|" + test.Content);
            }

            Console.WriteLine("---------------------------------------");
        }

        /// <summary>
        /// This function is used to verify whether numbers(And only) numbers are used in number fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NumberVerification(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private List<string> PromptManager(string label, out bool required)
        {
            var dialog = new PromptForFields(label);
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
    }

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
            switch (field?.FieldType)
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