using Asset_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Asset_Management_System.ViewModels.Commands.ViewModelHelper;

namespace Asset_Management_System.ViewModels.Commands
{
    class AddFieldCommand : ICommand
    {
        private FieldsController _viewModel;
        public event EventHandler CanExecuteChanged;

        public AddFieldCommand(FieldsController viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            string fieldToAdd = parameter.ToString();

            
            // Getting label, default value and is required
            List<string> promptResults = new List<string>();
            int fieldType = 0;
            bool required;
            
            switch (fieldToAdd)
            {
                case "Text Field":
                    Console.WriteLine("Textfield added");
                    if ((promptResults = _viewModel.PromptManager("Text box", out required)).Count > 0)
                    {
                        fieldType = 1;
                    }
                    break;
                case "String Field":
                    Console.WriteLine("StringField added");
                    if ((promptResults = _viewModel.PromptManager("String Field", out required)).Count > 0)
                    {
                        fieldType = 2;
                    }
                    break;
                case "Integer Field":
                    Console.WriteLine("IntegerField added");
                    if ((promptResults = _viewModel.PromptManager("Integer Field", out required)).Count > 0)
                    {
                        fieldType = 3;
                    }
                    break;
                case "Date Field":
                    Console.WriteLine("Date Field added");
                    if ((promptResults = _viewModel.PromptManager("Date Field", out required)).Count > 0)
                    {
                        fieldType = 4;
                    }
                    break;
                case "Boolean Field":
                    Console.WriteLine("BooleanField added");
                    if ((promptResults = _viewModel.PromptManager("Boolean Field", out required)).Count > 0)
                    {
                        fieldType = 5;
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }
            if (fieldType != 0)
            {
                ShownField shownField = new ShownField();
                Field addedField = new Field(promptResults[0], promptResults[1], fieldType, promptResults[1],
                    required);
                shownField.Name = addedField.GetHashCode().ToString();
                shownField.Field = addedField;
                _viewModel.FieldsList.Add(shownField);
            }
        }
    }
}
