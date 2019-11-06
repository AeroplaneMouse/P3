﻿using Asset_Management_System.Models;
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
        private MainViewModel _main;
        private FieldsController _viewModel;
        private readonly bool _isCustom;

        public event EventHandler CanExecuteChanged;

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
                    _main.AddNotification(
                        new Notification("ERROR! Adding field failed. Received object is not a field.",
                            Notification.ERROR), 5000);
            }
        }
    }
}