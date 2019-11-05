using Asset_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Asset_Management_System.ViewModels.ViewModelHelper;
using Asset_Management_System.Events;

namespace Asset_Management_System.ViewModels.Commands
{
    class AddFieldCommand : ICommand
    {
        private FieldsController _viewModel;
        private MainViewModel _main;
        public event EventHandler CanExecuteChanged;

        private readonly bool _isCustom;


        public AddFieldCommand(MainViewModel main, FieldsController viewModel, bool isCustom = false)
        {
            _main = main;
            _viewModel = viewModel;
            this._isCustom = isCustom;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _main.DisplayPrompt(new Views.Prompts.CustomField(null, AddNewFieldConfirmed));



            Console.WriteLine("Not implemented. maybe old method ");
            //string fieldToAdd = parameter.ToString();


            //// Getting label, default value and is required
            //List<string> promptResults = _viewModel.PromptManager("Text box", out var required);
            //int fieldType = 0;
            //bool correctPrompt = promptResults.Count > 0;

            //switch (fieldToAdd)
            //{
            //    case "Text Field":
            //        Console.WriteLine("Textfield added");
            //        fieldType = 1;
            //        break;
            //    case "String Field":
            //        Console.WriteLine("StringField added");
            //        fieldType = 2;
            //        break;
            //    case "Integer Field":
            //        Console.WriteLine("IntegerField added");
            //        fieldType = 3;
            //        break;
            //    case "Date Field":
            //        Console.WriteLine("Date Field added");
            //        fieldType = 4;
            //        break;
            //    case "Boolean Field":
            //        Console.WriteLine("BooleanField added");
            //        fieldType = 5;
            //        break;
            //    default:
            //        throw new NotSupportedException();
            //}

            //if (fieldType != 0 && correctPrompt)
            //{
            //    string defaultValue = "";
            //    string content = "";
            //    if (_isCustom)
            //    {
            //        defaultValue = promptResults[1];
            //        content = promptResults[1];
            //    }
            //    else
            //    {
            //        defaultValue = promptResults[1];
            //    }

            //    ShownField shownField = new ShownField(new Field(promptResults[0], content, fieldType,
            //        defaultValue,
            //        required, _isCustom));
            //    _viewModel.FieldsList.Add(shownField);
            //}
        }

        private void AddNewFieldConfirmed(object sender, PromptEventArgs e)
        {
            if (e.Result)
            {
                if (e.ResultObject is Field newField)
                {
                    ShownField shownField = new ShownField(newField);
                    _viewModel.FieldsList.Add(shownField);
                }
                else
                    _main.AddNotification(new Notification("ERROR! Adding field failed. Received object is not a field.", Notification.ERROR), 5000);
            }
        }
    }
}