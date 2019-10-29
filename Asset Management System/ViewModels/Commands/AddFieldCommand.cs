using Asset_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            
            switch (fieldToAdd)
            {
                case "Text Field":
                    Console.WriteLine("Textfield added");
                    if ((promptResults = _viewModel.PromptManager("Text box", out bool required)).Count > 0)
                    {
                        _viewModel.FieldsList.Add(new Field(promptResults[0], promptResults[1], 1, promptResults[1],
                            required));
                    }
                    break;
                case "String Field":
                    Console.WriteLine("StringField added");
                    if ((promptResults = _viewModel.PromptManager("String Field", out required)).Count > 0)
                    {
                        _viewModel.FieldsList.Add(new Field(promptResults[0], promptResults[1], 2, promptResults[1],
                            required));
                    }
                    break;
                case "Integer Field":
                    Console.WriteLine("IntegerField added");
                    if ((promptResults = _viewModel.PromptManager("Integer Field", out required)).Count > 0)
                    {
                        _viewModel.FieldsList.Add(new Field(promptResults[0], promptResults[1], 3, promptResults[1],
                            required));
                    }
                    break;
                case "Date Field":
                    Console.WriteLine("Date Field added");
                    if ((promptResults = _viewModel.PromptManager("Date Field", out required)).Count > 0)
                    {
                        _viewModel.FieldsList.Add(new Field(promptResults[0], promptResults[1], 4, promptResults[1],
                            required));
                    }
                    break;
                case "Boolean Field":
                    Console.WriteLine("BooleanField added");
                    if ((promptResults = _viewModel.PromptManager("Boolean Field", out required)).Count > 0)
                    {
                        _viewModel.FieldsList.Add(new Field(promptResults[0], promptResults[1], 5, promptResults[1],
                            required));
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
