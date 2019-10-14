using System;
using System.Collections.Generic;
using System.Text;
using Asset_Management_System.Database.Repositories;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace Asset_Management_System.Commands.Asset
{
    internal class SearchAssetsCommand : ICommand
    {
        private ViewModels.AssetsViewModel _view;

        public SearchAssetsCommand(ViewModels.AssetsViewModel view)
        {
            _view = view;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Console.WriteLine($"Searching for assets: { _view.SearchQueryText }");
            //ObservableCollection<Models.Asset> assets = new AssetRepository().Search(_view.SearchQueryText);
            //_view.Assets = assets;
        }
    }
}
