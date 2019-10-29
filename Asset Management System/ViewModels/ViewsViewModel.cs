using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels
{
    class ViewsViewModel : Base.BaseViewModel
    {
        private MainViewModel _main;

        public ViewsViewModel(MainViewModel main)
        {
            _main = main;


            CancelCommand = new Base.RelayCommand(() => _main.ChangeMainContent(new Views.Assets(main)));
        }


        public ICommand CancelCommand { get; set; }




    }
}
