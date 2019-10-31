using Asset_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Asset_Management_System.ViewModels.ViewModelHelper;

namespace Asset_Management_System.ViewModels.Commands
{
    class AddFieldCommand : ICommand
    {
        private FieldsController _viewModel;
        public event EventHandler CanExecuteChanged;

        private readonly bool _isCustom;

        public AddFieldCommand(FieldsController viewModel, bool isCustom = false)
        {
            _viewModel = viewModel;
            this._isCustom = isCustom;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            string fieldToAdd = parameter.ToString();


            // Getting label, default value and is required
            List<string> promptResults = _viewModel.PromptManager("Text box", out var required);
            int fieldType = 0;
            bool correctPrompt = promptResults.Count > 0;

            switch (fieldToAdd)
            {
                case "Text Field":
                    Console.WriteLine("Textfield added");
                    fieldType = 1;
                    break;
                case "String Field":
                    Console.WriteLine("StringField added");
                    fieldType = 2;
                    break;
                case "Integer Field":
                    Console.WriteLine("IntegerField added");
                    fieldType = 3;
                    break;
                case "Date Field":
                    Console.WriteLine("Date Field added");
                    fieldType = 4;
                    break;
                case "Boolean Field":
                    Console.WriteLine("BooleanField added");
                    fieldType = 5;
                    break;
                default:
                    throw new NotSupportedException();
            }

            if (fieldType != 0 && correctPrompt)
            {
                ShownField shownField = new ShownField(new Field(promptResults[0], promptResults[1], fieldType,
                    promptResults[1],
                    required, _isCustom));
                _viewModel.FieldsList.Add(shownField);
            }
        }
    }
}