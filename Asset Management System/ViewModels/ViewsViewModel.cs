using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Asset_Management_System.Services.Interfaces;

namespace Asset_Management_System.ViewModels
{
    class ViewsViewModel : Base.BaseViewModel
    {
        private MainViewModel _main;
        private IAssetService _service;
        
        public ViewsViewModel(MainViewModel main, IAssetService service)
        {
            _main = main;
            _service = service;

            CancelCommand = new Base.RelayCommand(() => _main.ChangeMainContent(new Views.Assets(main, _service)));
        }

        public ICommand CancelCommand { get; set; }
    }
}