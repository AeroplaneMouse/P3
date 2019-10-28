using Asset_Management_System.Models;
using Asset_Management_System.Views;
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
            AddFieldCommand = new Commands.AddFieldCommand(this);
            RemoveFieldCommand = new Commands.RemoveFieldCommand(this);
            CancelCommand = new Base.RelayCommand(() => _main.ChangeMainContent(new Assets(main)));
            AddFieldTestCommand = new Base.RelayCommand(() => AddField());
            AddFieldTest2Command = new Base.RelayCommand(() => AddField2());
        }

        private void AddField()
        {
            Field field = new Field("Test field", "", 1, "", false);
            FieldsList.Add(field);
            Console.WriteLine("Field added");
        }

        private void AddField2()
        {
            Field field = new Field("Test field 2", "", 2, "", false);
            FieldsList.Add(field);
            Console.WriteLine("Field 2 added");
        }

        public ICommand SaveAssetCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public static ICommand RemoveFieldCommand { get; set; }

        public ICommand AddFieldTestCommand { get; set; }
        public ICommand AddFieldTest2Command { get; set; }

        public bool CanSaveAsset()
        {
            // **** TODO ****
            // Only return true, if the entered values are valid.

            foreach (Field field in FieldsList)
            {
                if (field.Required && field.Content == String.Empty)
                    return false;
            }

            return true;
        }


        protected override void LoadFields()
        {
            _asset.DeserializeFields();
            foreach (var field in _asset.FieldsList)
                FieldsList.Add(field);

            Name = _asset.Name;
            Description = _asset.Description;

            // Notify view about changes
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Description));
        }
    }
}
