using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using Asset_Management_System.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Asset_Management_System.Logging;
using Asset_Management_System.Services.Interfaces;
using Asset_Management_System.Events;

namespace Asset_Management_System.ViewModels.Commands
{
    class SaveAssetCommand : ICommand
    {
        private MainViewModel _main;
        private Asset _asset;
        private IAssetService _service;
        private IAssetRepository _rep;
        private bool _editing;
        private bool _multipleSave;
        private ObservableCollection<ITagable> _CurrentlyAddedTags;

        public event EventHandler CanExecuteChanged;

        public SaveAssetCommand(MainViewModel main, Asset asset, IAssetService service, ObservableCollection<ITagable> CurrentlyAddedTags,bool editing, bool multipleSave = false)
        {
            _main = main;
            _asset = asset;
            _service = service;
            _rep = _service.GetSearchableRepository() as IAssetRepository;
            _editing = editing;
            _multipleSave = multipleSave;
            _CurrentlyAddedTags = CurrentlyAddedTags;
        }


        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (_main.CurrentDepartment != null && _main.CurrentDepartment.ID != 0)
            {
                // Prompt for approval if saving multiple
                if (_multipleSave)
                    _main.DisplayPrompt(new Views.Prompts.Confirm("Saving copy. Are you sure?", SaveApproved));
                else
                    SaveApproved(this, new PromptEventArgs(true));
            }
            else
            {
                const string message = "ERROR! No department set. Please create a department to attach the asset to.";
                _main.AddNotification(new Notification(message, Notification.ERROR));
            }
        }

        private void SaveApproved(object sender, PromptEventArgs e)
        {
            if (e.Result)
            {
                if (_asset.Identifier == null)
                    _asset.Identifier = "";

                
            
                // Checks if Name are not empty
                if (string.IsNullOrEmpty(_asset.Name))
                {
                   return;
                }


                // Update asset if editing, otherwise insert new.
                if (_editing)
                {
                    Log<Asset>.CreateLog(_asset);
                    _rep.Update(_asset);
                    if (_CurrentlyAddedTags.Count > 0)
                        _rep.AttachTags(_asset, new List<ITagable>(_CurrentlyAddedTags));
                }
                else
                {
                    _rep.Insert(_asset, out ulong id);
                    Log<Asset>.CreateLog(_asset, id);
                    if (_CurrentlyAddedTags.Count > 0)
                        _rep.AttachTags(_rep.GetById(id), new List<ITagable>(_CurrentlyAddedTags));
                }

                _main.AddNotification(new Notification("Asset saved to database", Notification.APPROVE));

                // Change page if save single
                if (!_multipleSave)
                    _main.ChangeMainContent(new Assets(_main, _service));
            }
        }
    }
}