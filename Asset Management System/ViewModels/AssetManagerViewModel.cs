using Asset_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels
{
    public class AssetManagerViewModel : FieldsController
    {
        private MainViewModel _main;
        private Asset _asset;

        public string Name { get; set; }
        public string Description { get; set; }


        public AssetManagerViewModel(MainViewModel main, Asset inputAsset)
        {
            _main = main;

            FieldsList = new ObservableCollection<Field>();
            if (inputAsset != null)
            {
                _asset = inputAsset;
                LoadFields();
                _editing = true;
            }
            else
            {
                _asset = new Asset();
                _editing = false;
            }



            // Initialize commands
            SaveAssetCommand = new Commands.SaveAssetCommand(this, _main, _asset, _editing);

        }

        public ICommand SaveAssetCommand { get; set; }
        public ICommand DeleteFieldCommand { get; set; }
        public ICommand AddFieldCommand { get; set; }

        public bool CanSaveAsset()
        {
            // **** TODO ****
            // Only return true, if the entered values are valid.
            return true;
        }


        private bool LoadFields()
        {
            ConsoleWriter.ConsoleWrite("------Field labels | Field content -------");
            _asset.DeserializeFields();
            foreach (var field in _asset.FieldsList)
            {
                ConsoleWriter.ConsoleWrite(field.Label + " | " + field.Content);
                FieldsList.Add(field);
            }
            Name = _asset.Name;
            Description = _asset.Description;

            // Notify view about changes
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Description));


            return true;
        }
    }
}
