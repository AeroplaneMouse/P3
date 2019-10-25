﻿using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels.Commands
{
    class SaveAssetCommand : ICommand
    {
        private AssetManagerViewModel _viewModel;
        private MainViewModel _main;
        private Asset _asset;
        private bool _editing;

        public event EventHandler CanExecuteChanged;

        public SaveAssetCommand(AssetManagerViewModel viewModel, MainViewModel main, Asset asset, bool editing)
        {
            _viewModel = viewModel;
            _main = main;
            _asset = asset;
            _editing = editing;
        }


        public bool CanExecute(object parameter)
        {
            return _viewModel.CanSaveAsset();
        }

        public void Execute(object parameter)
        {

            _asset.Name = _viewModel.Name;
            _asset.Description = _viewModel.Description;

            _asset.FieldsList = new List<Field>();
            foreach (var field in _viewModel.FieldsList)
            {
                if (field.Required && field.Content == string.Empty)
                {
                    _main.AddNotification(new Notification("ERROR! A required field wasn't filled.", Notification.ERROR));
                    return;
                    //requiredFieldsWritten = false;
                }
                _asset.AddField(field);
            }

            Department department = _main.CurrentDepartment;
            if (department != null)
            {
                _asset.DepartmentID = department.ID;
                // Creates a log entry, currently uses for testing.
                //_asset.Notify();
                AssetRepository rep = new AssetRepository();

                if (_editing)
                    rep.Update(_asset);
                else
                    rep.Insert(_asset);

                _main.ChangeMainContent(new Assets(_main));
            }
            else
            {
                const string message = "ERROR! No department set. Please create a department to attach the asset to.";
                _main.AddNotification(new Notification(message, Notification.ERROR));
            }
        }
    }
}