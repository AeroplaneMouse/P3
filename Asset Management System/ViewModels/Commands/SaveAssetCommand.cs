using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Asset_Management_System.Logging;
using Asset_Management_System.Services.Interfaces;

namespace Asset_Management_System.ViewModels.Commands
{
    class SaveAssetCommand : ICommand
    {
        private AssetManagerViewModel _viewModel;
        private MainViewModel _main;
        private Asset _asset;
        private IAssetService _service;
        private IAssetRepository _rep;
        private bool _editing;
        private bool _multipleSave;

        public event EventHandler CanExecuteChanged;

        public SaveAssetCommand(AssetManagerViewModel viewModel, MainViewModel main, Asset asset, IAssetService service, bool editing,bool multipleSave = false)
        {
            _viewModel = viewModel;
            _main = main;
            _asset = asset;
            _service = service;
            _rep = _service.GetSearchableRepository() as IAssetRepository;
            _editing = editing;
            _multipleSave = multipleSave;
        }


        public bool CanExecute(object parameter)
        {
            return _viewModel.CanSaveAsset();
        }

        public void Execute(object parameter)
        {
            if (_main.CurrentDepartment.ID == 0)
            {
                _main.AddNotification(new Notification("Please select a department.", Notification.ERROR));
                return;
            }
            _asset.Name = _viewModel.Name;
            _asset.Description = _viewModel.Description;

            _asset.Identifier = _viewModel.Identifier;
            if (_asset.Identifier == null)
                _asset.Identifier = "";

            _asset.FieldsList = new List<Field>();
            // Checks if Name or Description is not empty.
            if (string.IsNullOrEmpty(_asset.Name))
            {
                _main.AddNotification(new Notification("ERROR! A required field wasn't filled.", Notification.ERROR));
                return;
            }

            foreach (var shownField in _viewModel.FieldsList)
            {
                if (shownField.Field.Required && shownField.Field.Content == string.Empty && !shownField.Field.IsHidden)
                {
                    _main.AddNotification(new Notification("ERROR! A required field wasn't filled.", Notification.ERROR));
                    return;
                }

                _asset.AddField(shownField.Field);
            }

            foreach (var shownField in _viewModel.HiddenFields)
            {
                _asset.AddField(shownField.Field);
            }

            Department department = _main.CurrentDepartment;

            if (department != null)
            {
                _asset.DepartmentID = department.ID;

                if (_editing)
                {
                    Log<Asset>.CreateLog(_asset);
                    _rep.Update(_asset);
                    if (_viewModel.CurrentlyAddedTags.Count > 0)
                    {
                        _rep.AttachTagsToAsset(_asset, new List<Tag>(_viewModel.CurrentlyAddedTags));
                    }
                }
                else
                {
                    _rep.Insert(_asset, out ulong id);
                    Log<Asset>.CreateLog(_asset, id);
                    if (_viewModel.CurrentlyAddedTags.Count > 0)
                    {
                        _rep.AttachTagsToAsset(_rep.GetById(id), new List<Tag>(_viewModel.CurrentlyAddedTags));
                    }
                }

                if (_multipleSave)
                {
                    _main.AddNotification(new Notification("Asset saved to database",Notification.APPROVE));
                }
                else
                {
                    _main.ChangeMainContent(new Assets(_main, _service));
                }
                
            }
            else
            {
                const string message = "ERROR! No department set. Please create a department to attach the asset to.";
                _main.AddNotification(new Notification(message, Notification.ERROR));
            }
        }
    }
}